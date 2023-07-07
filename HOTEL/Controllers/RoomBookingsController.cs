using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using HOTEL.Models;
using HOTEL.ViewModel;
using MailKit.Security;
using MimeKit;

namespace HOTEL.Controllers
{
    public class RoomBookingsController : Controller
    {
        private HotelDBEntities db = new HotelDBEntities();

        // GET: RoomBookings
        public ActionResult Index(string searchString, string bookingId, string customerNumber)
        {
            var bookings = db.RoomBookings.Include(r => r.PaymentType);

            if (User.IsInRole("Admin"))
            {
                if (!string.IsNullOrEmpty(searchString))
                {
                    bookings = bookings.Where(b =>
                        b.CustomerName.Contains(searchString) ||
                        b.CustomerEmail.Contains(searchString) ||
                        b.CustomerNumber.Contains(searchString) ||
                        b.Comments.Contains(searchString) ||
                        b.AssignRoomId.ToString().Contains(searchString));
                }

                if (!string.IsNullOrEmpty(bookingId))
                {
                    int id = int.Parse(bookingId);
                    bookings = bookings.Where(b => b.BookingId == id);
                }

                if (!string.IsNullOrEmpty(customerNumber))
                {
                    bookings = bookings.Where(b => b.CustomerNumber == customerNumber);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(searchString) || !string.IsNullOrEmpty(bookingId) || !string.IsNullOrEmpty(customerNumber))
                {
                    if (!string.IsNullOrEmpty(searchString))
                    {
                        bookings = bookings.Where(b =>
                            b.CustomerName.Contains(searchString) ||
                            b.CustomerEmail.Contains(searchString) ||
                            b.CustomerNumber.Contains(searchString) ||
                            b.Comments.Contains(searchString) ||
                            b.AssignRoomId.ToString().Contains(searchString));
                    }

                    if (!string.IsNullOrEmpty(bookingId))
                    {
                        int id = int.Parse(bookingId);
                        bookings = bookings.Where(b => b.BookingId == id);
                    }

                    if (!string.IsNullOrEmpty(customerNumber))
                    {
                        bookings = bookings.Where(b => b.CustomerNumber == customerNumber);
                    }
                }
                else
                {
                    bookings = Enumerable.Empty<RoomBookings>().AsQueryable();
                }
            }

            bookings = bookings.OrderByDescending(b => b.BookingId);
            return View(bookings.ToList());
        }

        // GET: RoomBookings/Create
        public ActionResult Create(int roomId, string DateFrom, string DateTo, int? roomPrice, string person)
        {
            DateTime datefrom = DateTime.Today.AddDays(1);
            string dateFromText = datefrom.ToString("d MMMM");
            DateTime dateto = DateTime.Today.AddDays(3);
            string dateToText = dateto.ToString("d MMMM");

            if (DateFrom != null)
                DateTime.TryParseExact(DateFrom, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out datefrom);
                dateFromText = datefrom.ToString("d MMMM");

            if (DateTo != null)
                DateTime.TryParseExact(DateTo, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateto);
                dateToText = dateto.ToString("d MMMM");

            
            

            TimeSpan span = dateto - datefrom;
            int days = span.Days;

            int totalPrice = roomPrice.Value * days;

            ViewBag.ID = roomId;

            ViewBag.DF = datefrom;
            ViewBag.DT = dateto;
            ViewBag.DFtxt = dateFromText;
            ViewBag.DTtxt = dateToText;
            ViewBag.PE = person;
            ViewBag.RP = roomPrice;
            ViewBag.days = days;
            ViewBag.totalPrice = totalPrice;
            
            return View();
        }

        // POST: RoomBookings/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BookingId,CustomerName,CustomerNumber,CustomerEmail,CustomerAdress,CustomerName1,CustomerName2,CustomerName3,BookingFrom,BookingTo,AssignRoomId,NoOfMembers,TotalAmount,PaymentTypeId,ServiceId,Comments,Confirmed")] RoomBookings roomBookings)
        {
            if (ModelState.IsValid)
            {
                
                // создаем новую запись
                db.RoomBookings.Add(roomBookings);
                

                db.SaveChanges();

                // Проверяем наличие адреса электронной почты
                if (!string.IsNullOrEmpty(roomBookings.CustomerEmail))
                {
                    try
                    {
                        // Отправляем письмо
                        MailMessage message = new MailMessage();
                        message.From = new MailAddress("vil_2003@mail.ru");
                        message.To.Add(roomBookings.CustomerEmail);
                        message.Subject = "Данные бронирования";
                        message.Body = $"Уважаемый {roomBookings.CustomerName}," +
                            $"\n\nБлагодарим, за то, что бронируете у нас. Прилагаем ваши детали бронирования:" +
                            $"\n\nНомер бронирования: {roomBookings.BookingId}" +
                            $"\nСтоимость: {roomBookings.TotalAmount}" +
                            $"\nЗаселение: {roomBookings.BookingFrom}" +
                            $"\nВыезд: {roomBookings.BookingTo}" +
                            $"\nНочей: {roomBookings.NoOfMembers}" +
                            $"\n\nМы надеемся ваше прибывание будет комфортным у нас.\n\nС наилучшими пожеланиями,\nАдминистрация отеля";

                        // Задаем SMTP-сервер и его порт
                        SmtpClient smtp = new SmtpClient("smtp.mail.ru", 587);
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential("vil_2003@mail.ru", "iA7RuDeFW8dBTtiUNqqn");
                        smtp.EnableSsl = true;

                        smtp.Send(message);
                    }
                    catch (Exception ex)
                    {
                        // Обрабатываем ошибки при отправке сообщения
                        ModelState.AddModelError("", "Ошибка при отправке сообщения: " + ex.Message);
                        return View(roomBookings);
                    }
                }

                return RedirectToAction("Index", "Home");
            }

            return View(roomBookings);
        }

        // GET: RoomBookings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomBookings roomBookings = db.RoomBookings.Find(id);

            List<SelectListItem> confirmedList = new List<SelectListItem>
    {
        new SelectListItem { Value = "1", Text = "Новое" },
        new SelectListItem { Value = "2", Text = "Подтверждено" },
        new SelectListItem { Value = "3", Text = "В работе" },
        new SelectListItem { Value = "4", Text = "Завершено" },
        new SelectListItem { Value = "5", Text = "Отменено" }
    };

            // Передаем список в ViewBag
            ViewBag.ConfirmedList = confirmedList;

            if (roomBookings == null)
            {
                return HttpNotFound();
            }
            return View(roomBookings);
        }

        // POST: RoomBookings/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BookingId,CustomerName,CustomerNumber,CustomerEmail,CustomerAdress,BookingFrom,BookingTo,AssignRoomId,NoOfMembers,TotalAmount,PaymentTypeId,ServiceId,Comments,Confirmed")] RoomBookings roomBookings)
        {
            if (ModelState.IsValid)
            {
                db.Entry(roomBookings).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(roomBookings);
        }

        // GET: RoomBookings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomBookings roomBookings = db.RoomBookings.Find(id);
            if (roomBookings == null)
            {
                return HttpNotFound();
            }
            return View(roomBookings);
        }

        // POST: RoomBookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RoomBookings roomBookings = db.RoomBookings.Find(id);
            db.RoomBookings.Remove(roomBookings);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using HOTEL.Models;
using HOTEL.ViewModel;


namespace HOTEL.Controllers
{
    public class RoomsController : Controller
    {
        public HotelDBEntities db = new HotelDBEntities();

        public ActionResult GetImage(int id, int imageNumber)
        {
            var room = db.Rooms.Find(id);

            byte[] imageBytes = null;
            switch (imageNumber)
            {
                case 1:
                    imageBytes = room.RoomImage1;
                    break;
                case 2:
                    imageBytes = room.RoomImage2;
                    break;
                case 3:
                    imageBytes = room.RoomImage3;
                    break;
                case 4:
                    imageBytes = room.RoomImage4;
                    break;
                case 5:
                    imageBytes = room.RoomImage5;
                    break;
            }

            if (imageBytes != null)
            {
                return File(imageBytes, "image/jpeg");
            }
            else
            {
                return null;
            }
        }

        public ActionResult ListRooms()
        {
            var availableRooms = from r in db.Rooms
                                 
                                 group r by r.RoomTypes.RoomTypeName into g
                                 select g.FirstOrDefault();

            return View(availableRooms.ToList());

        }



        // GET: Rooms/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var room = from r in db.Rooms
                                 where r.RoomId==id
                                 select r;
            if (room == null)
            {
                return HttpNotFound();
            }
            return View(room.ToList());
        }



        public ActionResult Index(string Person, string DateFrom = null, string DateTo = null)
        {
            DateTime datefrom = DateTime.Today.AddDays(1);
            DateTime dateto = DateTime.Today.AddDays(3);


            if (!string.IsNullOrEmpty(DateFrom))
                DateTime.TryParseExact(DateFrom, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out datefrom);

            if (!string.IsNullOrEmpty(DateTo))
                DateTime.TryParseExact(DateTo, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateto);

            ViewData["DateFrom"] = datefrom.ToString("yyyy-MM-dd");
            ViewData["DateTo"] = dateto.ToString("yyyy-MM-dd");
            ViewData["Person"] = Person;

            if (User.IsInRole("Admin"))
            {
                var allRooms = from r in db.Rooms
                               select r;

                return View(allRooms.ToList());
            }
            else
            {
                int PE = Convert.ToInt32(Person);

                var availableRooms = from r in db.Rooms
                                     where r.RoomCapacity >= PE &&
                                          !db.RoomBookings.Any(b => b.AssignRoomId == r.RoomId &&
                                         ((b.BookingFrom <= datefrom && b.BookingTo > datefrom) ||
                                         (b.BookingFrom < dateto && b.BookingTo >= dateto) ||
                                         (b.BookingFrom >= datefrom && b.BookingTo <= dateto)))
                                     select r;

                return View(availableRooms.ToList());
            }
        }

        // GET: Rooms/Create
        public ActionResult Create()
        {
            RoomViewModel objRoomViewModel = new RoomViewModel();
            objRoomViewModel.ListOfRoomType = (from obj in db.RoomTypes
                                               select new SelectListItem()
                                               {
                                                   Text = obj.RoomTypeName,
                                                   Value = obj.RoomTypeId.ToString()
                                               }).ToList();
            return View(objRoomViewModel);
        }

        // POST: Rooms/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RoomId,RoomNumber,RoomPrice,RoomTypeId,RoomCapacity,Square,RoomDescription,RoomImage1,RoomImage2,RoomImage3,RoomImage4,RoomImage5")] Rooms room, HttpPostedFileBase upload0, HttpPostedFileBase upload1, HttpPostedFileBase upload2, HttpPostedFileBase upload3, HttpPostedFileBase upload4)
        {
            if (ModelState.IsValid)
            {
                if (upload0 != null && upload0.ContentLength > 0)
                {
                    using (var reader = new System.IO.BinaryReader(upload0.InputStream))
                    {
                        room.RoomImage1 = reader.ReadBytes(upload0.ContentLength);
                    }
                }
                if (upload1 != null && upload1.ContentLength > 0)
                {
                    using (var reader = new System.IO.BinaryReader(upload1.InputStream))
                    {
                        room.RoomImage2 = reader.ReadBytes(upload1.ContentLength);
                    }
                }
                if (upload2 != null && upload2.ContentLength > 0)
                {
                    using (var reader = new System.IO.BinaryReader(upload2.InputStream))
                    {
                        room.RoomImage3 = reader.ReadBytes(upload2.ContentLength);
                    }
                }
                if (upload3 != null && upload3.ContentLength > 0)
                {
                    using (var reader = new System.IO.BinaryReader(upload3.InputStream))
                    {
                        room.RoomImage4 = reader.ReadBytes(upload3.ContentLength);
                    }
                }
                if (upload4 != null && upload4.ContentLength > 0)
                {
                    using (var reader = new System.IO.BinaryReader(upload4.InputStream))
                    {
                        room.RoomImage5 = reader.ReadBytes(upload4.ContentLength);
                    }
                }

                db.Rooms.Add(room);

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(room);
        }

        public ActionResult Edit(int? id, int? roomId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Rooms, RoomViewModel>();
            });
            var mapper = config.CreateMapper();

            var room = db.Rooms.Find(id);
            var roomViewModel = mapper.Map<RoomViewModel>(room);

            // Заполнение списка ListOfRoomType
            roomViewModel.ListOfRoomType = db.RoomTypes.Select(x => new SelectListItem
            {
                Value = x.RoomTypeId.ToString(),
                Text = x.RoomTypeName
            }).ToList();

            if (room == null)
            {
                return HttpNotFound();
            }

            return View(roomViewModel);
        }

        // POST: Rooms/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RoomId,RoomNumber,RoomPrice,RoomTypeId,RoomCapacity,Square,RoomDescription,RoomImage1,RoomImage2,RoomImage3,RoomImage4,RoomImage5")] Rooms room, HttpPostedFileBase[] uploads)
        {
            if (ModelState.IsValid)
            {
                for (int i = 0; i < uploads.Length; i++)
                {
                    HttpPostedFileBase upload = uploads[i];
                    if (upload != null && upload.ContentLength > 0)
                    {
                        using (var reader = new System.IO.BinaryReader(upload.InputStream))
                        {
                            switch (i)
                            {
                                case 0:
                                    room.RoomImage1 = reader.ReadBytes(upload.ContentLength);
                                    break;
                                case 1:
                                    room.RoomImage2 = reader.ReadBytes(upload.ContentLength);
                                    break;
                                case 2:
                                    room.RoomImage3 = reader.ReadBytes(upload.ContentLength);
                                    break;
                                case 3:
                                    room.RoomImage4 = reader.ReadBytes(upload.ContentLength);
                                    break;
                                case 4:
                                    room.RoomImage5 = reader.ReadBytes(upload.ContentLength);
                                    break;
                            }
                        }
                    }
                }

                db.Entry(room).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(room);
        }

        // GET: Rooms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rooms room = db.Rooms.Find(id);
            if (room == null)
            {
                return HttpNotFound();
            }
            return View(room);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Rooms room = db.Rooms.Find(id);
            db.Rooms.Remove(room);
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

    internal interface IFormFile
    {
    }

    public interface IFormFileCollection
    {
    }
}

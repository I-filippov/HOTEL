using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HOTEL.Models;

namespace HOTEL.Controllers
{
    public class RoomTypesController : Controller
    {
        private HotelDBEntities db = new HotelDBEntities();

        // GET: RoomTypes
        public ActionResult Index()
        {
            return View(db.RoomTypes.ToList());
        }

        // GET: RoomTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RoomTypes/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RoomTypeId,RoomTypeName")] RoomTypes roomTypes)
        {
            if (ModelState.IsValid)
            {
                db.RoomTypes.Add(roomTypes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(roomTypes);
        }

        // GET: RoomTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomTypes roomTypes = db.RoomTypes.Find(id);
            if (roomTypes == null)
            {
                return HttpNotFound();
            }
            return View(roomTypes);
        }

        // POST: RoomTypes/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RoomTypeId,RoomTypeName")] RoomTypes roomTypes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(roomTypes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(roomTypes);
        }

        // GET: RoomTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomTypes roomTypes = db.RoomTypes.Find(id);
            if (roomTypes == null)
            {
                return HttpNotFound();
            }
            return View(roomTypes);
        }

        // POST: RoomTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RoomTypes roomTypes = db.RoomTypes.Find(id);
            db.RoomTypes.Remove(roomTypes);
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

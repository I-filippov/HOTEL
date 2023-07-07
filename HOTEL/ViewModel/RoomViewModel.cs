using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HOTEL.ViewModel
{
    public class RoomViewModel
    {
        [Display(Name = "Id комнаты")]
        public int RoomId { get; set; }

        [Display(Name = "Номер комнаты")]
        public string RoomNumber { get; set; }

        [Display(Name = "Цена за ночь")]
        public int RoomPrice { get; set; }

        [Display(Name = "Тип комнаты")]
        public int RoomTypeId { get; set; }

        [Display(Name = "Вместимость комнаты")]
        public int RoomCapacity { get; set; }

        [Display(Name = "Описание")]
        public string RoomDescription { get; set; }

        [Display(Name = "Площадь")]
        public int Square { get; set; }

        [Display(Name = "Фото 1")]
        public byte[] RoomImage1 { get; set; }

        [Display(Name = "Фото 2")]
        public byte[] RoomImage2 { get; set; }

        [Display(Name = "Фото 3")]
        public byte[] RoomImage3 { get; set; }

        [Display(Name = "Фото 4")]
        public byte[] RoomImage4 { get; set; }

        [Display(Name = "Фото 5")]
        public byte[] RoomImage5 { get; set; }

        public List<SelectListItem> ListOfRoomType { get; set; }
        public List<byte[]> Images { get; internal set; }
    }
}
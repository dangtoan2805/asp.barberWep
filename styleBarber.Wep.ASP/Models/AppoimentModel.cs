﻿using AutoMapper;
using styleBarber.Wep.ASP.Dao;
using styleBarber.Wep.ASP.Entities;
using styleBarber.Wep.ASP.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace styleBarber.Wep.ASP.Models
{
    public class AppoimentModel
    {
        private AppoimentDao _db = null;
        private const string appointmentKey = "appointment";
        private const string countAppoitment = "count";
        private bool _isModified = true;
        private IMapper _mapper = null;
        public AppoimentModel()
        {
            _db = new AppoimentDao();
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Appointment, AppointmentVM>()
                    .ForMember(des => des.BarberName, act => act.MapFrom(src => src.Barber.Name))
                    .ForMember(des => des.DateCheckOut, act => act.MapFrom(src => src.Date.ToShortDateString()))
                    .ForMember(des => des.Time, act => act.MapFrom(src => src.Date.ToShortTimeString()));
                cfg.CreateMap<AppointmentVM, Appointment>()
                    .ForMember(des => des.Date, act => act.MapFrom(src => Convert.ToDateTime(src.DateCheckOut + src.Time)));
            }).CreateMapper();
        }

        private List<AppointmentVM> GetData()
        {
            _isModified = false;
            var rawData = _db.GetAppointments();
            List<AppointmentVM> appointmentVMs = new List<AppointmentVM>();
            if (rawData.Count ==0) return appointmentVMs;
            foreach (var item in rawData)
                appointmentVMs.Add(_mapper.Map<AppointmentVM>(item));
            DataCache.SetInCache(appointmentKey, appointmentVMs);
            return appointmentVMs;
        }

        private int Count()
        {
            _isModified = false;
            var rawTotal = _db.Count();
            DataCache.SetInCache(countAppoitment, rawTotal);
            return rawTotal;
        }
        public int CountTotal()
        {
            if (_isModified)
                return Count();
            var data = DataCache.GetInCache<int>(countAppoitment);
            return data;        
        }

        public List<AppointmentVM> GetAppointmentVMs()
        {
            if (_isModified)
                return GetData();
            var data = DataCache.GetInCache<List<AppointmentVM>>(appointmentKey);
            if (data == null)
                return GetData();
            return data;
        }

        public List<AppointmentVM> FilterByDate(DateTime date)
        {
            return GetAppointmentVMs()
                .Where(item => Convert.ToDateTime(item.DateCheckOut).Date == date.Date)
                .ToList();
        }

        public List<AppointmentVM> FilterByWeek(DateTime start, DateTime  end)
        {
            return GetAppointmentVMs()
                .Where(item => Convert.ToDateTime(item.DateCheckOut).Date >= start.Date && Convert.ToDateTime(item.DateCheckOut).Date <= end.Date.AddDays(1))
                .ToList();
        }

        public void Add(AppointmentVM appointment)
        {
            if (appointment == null) return;
            _isModified = true;
            _db.Add(_mapper.Map<Appointment>(appointment));
        }

        public void Update(int id, bool status)
        {
            if (id == 0) return;
            _isModified = true;
            _db.UpdateStatus(id, status);
        }

        public List<string> GetTimes()
        {
            return new List<string>()
            {
                "8:00",
                "9:00",
                "10:00",
                "11:00",
                "12:00",
                "13:00",
            };
        }


    }
    public class AppointmentVM
    {
        public int ID { get; set; }
        public string DateCheckOut { get; set; }
        public string Time { get; set; }
        public string FisrtName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Note { get; set; }
        public string BarberName { get; set; }
        public int BarberID { get; set; }
    }
}
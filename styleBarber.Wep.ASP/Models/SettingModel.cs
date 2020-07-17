﻿using styleBarber.Wep.ASP.Dao;
using styleBarber.Wep.ASP.Entities;
using styleBarber.Wep.ASP.Helper;
using System.Collections.Generic;

namespace styleBarber.Wep.ASP.Models
{
    public class SettingModel
    {
        private SettingDao _db = null;
        private const string infoKey = "info";
        private const string reviewerKey = "reviewer";
        private bool isModified = true;
        public SettingModel()
        {
            _db = new SettingDao();
        }

        private InfoStore GetInfoData()
        {
            isModified = false;
            var rawData = _db.GetInfo();
            if (rawData == null) return new InfoStore();
            DataCache.SetInCache(infoKey, rawData);
            return rawData;
        }

        private List<Reviewer> GetReviewerData()
        {
            isModified = false;
            var rawData = _db.GetReviewers();
            if (rawData == null) return rawData;
            DataCache.SetInCache(reviewerKey, rawData);
            return rawData;
        }

        public InfoStore GetInfo()
        {
            if (isModified)
                return GetInfoData();
            var data = DataCache.GetInCache<InfoStore>(infoKey);
            if (data == null)
                return GetInfoData();
            else
                return data;
        }

        public void UpdateInfo(InfoStore info)
        {
            if (info == null) return;
            _db.Update(info);
        }

        public List<Reviewer> GetReviewers()
        {
            if (isModified)
                return GetReviewerData();
            var data = DataCache.GetInCache<List<Reviewer>>(reviewerKey);
            if (data == null)
                return GetReviewerData();
            else
                return data;
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using VoteMap.Core.Models;
using VoteMap.Core.Utility;

namespace VoteMap.Core.Controllers
{
    public class HomeAPIController : Controller
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IAppUtility _util;
        private readonly ICSVUtility _csv;

        public HomeAPIController(IAppUtility util, ICSVUtility csv)
        {
            _util = util;
            _csv = csv;
        }

        public ActionResult GetTaiwanCityTopo()
        {
            // Data Source : https://kiang.github.io/taiwan_basecode/city/city.topo.json
            try
            {
                using (StreamReader r = new StreamReader(_util.GetDataPath("city.topo.json")))
                {
                    string json = r.ReadToEnd();
                    return Content(json, "application/json;charset=utf-8 ");
                }
            }
            catch (Exception e)
            {
                logger.Debug(e.StackTrace);
                logger.Error(e.Message);
                return Content("{}", "application/json;charset=utf-8 ");
            }

        }

        public ActionResult GetTownMapMetric(string year = "", string type = "")
        {
            VoteMapResp<ListResult<MapMetric>> resp = new VoteMapResp<ListResult<MapMetric>>() { retCode = 0, retMessage = "", result = new ListResult<MapMetric>() { list = new List<MapMetric>() } };
            try
            {
                resp.result.list = _csv.getDataFromCSV<MapMetric>(_util.GetDataPath($"{year}_{type}.csv"));
            }
            catch (Exception e)
            {
                logger.Debug(e.StackTrace);
                logger.Error(e.Message);
                resp.retCode = 1;
                resp.retMessage = e.Message;
                resp.requestID = Request.Headers.ContainsKey("requestid") ? Request.Headers["requestid"].ToString() : "";
            }
            return Content(_util.ConvertObj2String<VoteMapResp<ListResult<MapMetric>>>(resp), "application/json;charset=utf-8");
        }

        public ActionResult GetPartyInfo()
        {
            VoteMapResp<ListResult<PoliticalParty>> resp = new VoteMapResp<ListResult<PoliticalParty>>() { retCode = 0, retMessage = "", result = new ListResult<PoliticalParty>() { list = new List<PoliticalParty>() } };
            try
            {
                resp.result.list = _csv.getDataFromCSV<PoliticalParty>(_util.GetDataPath($"elpaty.csv"));
            }
            catch (Exception e)
            {
                logger.Debug(e.StackTrace);
                logger.Error(e.Message);
                resp.retCode = 1;
                resp.retMessage = e.Message;
                resp.requestID = Request.Headers.ContainsKey("requestid") ? Request.Headers["requestid"].ToString() : "";
            }
            return Content(_util.ConvertObj2String<VoteMapResp<ListResult<PoliticalParty>>>(resp), "application/json;charset=utf-8");
        }
    }
}
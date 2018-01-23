﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManageMuseum.Models;

namespace ManageMuseum.Controllers
{
    public class ExhibitionSheduleController : Controller
    {
        private OurContectDb db = new OurContectDb();
        // GET: SheduleExhibition
        public ActionResult SheduleExhibition()
        {
            // Defines past events as closed, and the rooms associated with those events are again free
            var now = DateTime.Now.Date;
            var getEventExhibitionState = db.EventStates.First(d => d.Name == "exibicao");
            var oldEventsOnExihibtion = db.Events.Include(d => d.EventState).Where(d => d.EnDate < now && d.EventState.Id == getEventExhibitionState.Id).ToList();
            var getRoomFreeState = db.SpaceStates.First(d => d.Name == "livre"); // Estado de sala livre
            var getEventFinishedState = db.EventStates.First(d => d.Name == "encerrado");
            foreach (var _event in oldEventsOnExihibtion)  // Coloca todos os eventos que o endDate já ocorreu, e que ainda se encontram em exibicao, com o estado encerrado
            {
                var getEventRooms = db.RoomMuseums.Include(d => d.Event).Where(d => d.Event.Id == _event.Id).ToList();
                foreach (var _room in getEventRooms) // coloca todas as salas associadas ao evento nas condicoes acima, com o estado de salas livres
                {
                    _room.SumRoomArtPieces = 0;
                    _room.SpaceState = getRoomFreeState;

                    db.SaveChanges();
                }
                _event.SumArtPieces = 0;
                _event.EventState = getEventFinishedState;
                db.SaveChanges();
            }
            
            var getListFreeRooms = db.RoomMuseums.Where(d => d.SpaceState.Name == getRoomFreeState.Name).ToList(); // Salas com o estado livre
            ViewBag.ListSpaces = new SelectList(getListFreeRooms, "Name", "Name");
            ViewBag.sizeListRooms = getListFreeRooms.Count;

            db.SaveChanges();

            return View();
        }
        [HttpPost]
        public ActionResult SheduleExhibition(EventViewModel events)
        {
            
            var queryListSpaces = db.RoomMuseums.ToList();
            ViewBag.ListSpaces = new SelectList(queryListSpaces, "Name", "Name");
           
            var rooms = events.SpacesList;
            var listSpaces = new List<RoomMuseum>();

            foreach (var items in rooms)
            {
                var query = db.RoomMuseums.Include(d => d.Event).Single(d => d.Name == items);
                listSpaces.Add(query);
            }
            var eventState = db.EventStates.Single(s => s.Name == "poraprovar");
            var eventType = db.EventTypes.Single(s => s.Name == "exposicao");
            var userId = Int32.Parse(Request.Cookies["UserId"].Value);
            var userAccont = db.UserAccounts.Single(s => s.Id == userId);
            var newEvent = new Event() { UserAccount = userAccont, Name = events.Name, EventState = eventState, EventType = eventType, Description = events.Description, StartDate = events.StartDate, EnDate = events.EnDate };

            foreach (var item in listSpaces)
            {
                item.Event = newEvent;
            }
            db.SaveChanges();
            return Redirect("SheduleExhibition");
           
            
        }

        public ActionResult ShowRequestsList()
        {
            var query = db.Events.Include(d => d.EventState).Include(d => d.EventType).Where(d => d.EventState.Name == "poraprovar").ToList();
            ViewBag.Data = query;
            return View();
        }

        public ActionResult EventRequestApprove(string eventId)
        {
            var EventIdApprove = Int32.Parse(eventId);

            EventState approvedState = db.EventStates.First(d => d.Name == "aceites");
            
            Event update = db.Events.Include(v => v.EventState).First(d => d.Id == EventIdApprove);
            update.EventState = approvedState;
            db.SaveChanges();
            //FALTA COLOCAR AQUI UMA MENSAGEM DE AVISO QUE O PEDIDO DE EVENTO FOI APROVADO
            return Redirect("ShowRequestsList");
        }

        public ActionResult EventRequestDetails(string eventId)
        {
            var EventIdSelected = Int32.Parse(eventId);

            var queryEventDetails = db.Events.Include(d=>d.UserAccount).Include(d => d.EventState).Include(d => d.EventType).Single(s => s.Id == EventIdSelected);
            ViewBag.evento = queryEventDetails;
            ViewData["EventUserId"] = queryEventDetails.UserAccount.Id;
            ViewData["EventUserFName"] = queryEventDetails.UserAccount.FirstName;
            ViewData["EventUserLName"] = queryEventDetails.UserAccount.LastName;
            ViewData["EventSelected"] = queryEventDetails.Id;
            ViewData["EventName"] = queryEventDetails.Name;
            ViewData["EventType"] = queryEventDetails.EventType.Name;
            ViewData["EventStartDate"] = queryEventDetails.StartDate;
            ViewData["EventEndDate"] = queryEventDetails.EnDate;
            ViewData["EventDescription"] = queryEventDetails.Description;

            return View();
        }

        public ActionResult EventRequestReject(string eventId)
        {
            var EventIdReject = Int32.Parse(eventId);

            EventState rejectState = db.EventStates.First(d => d.Name == "rejeitado");

            Event update = db.Events.Include(v => v.EventState).First(d => d.Id == EventIdReject);
            update.EventState = rejectState;
            db.SaveChanges();
            return Redirect("ShowRequestsList");
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManageMuseum.Models;


namespace ManageMuseum.Controllers
{
    public class CatalogController : Controller
    {
        

        private OurContectDb db = new OurContectDb();

        
        public ActionResult ListArtPieces()
        {
            var query = db.ArtPieces.Include(d => d.ArtPieceState).Include(v => v.RoomMuseum).ToList();
            ViewBag.Data = query;
            return View();
        }

        public ActionResult InsertArtPiece()
        {
            return View();
        }

        public ActionResult RemovePiece(string artpieceId)
        {
            var pieceId = Int32.Parse(artpieceId); // Convert artpieceId from string to int
            var getPieceStorageState = db.ArtPieceStates.Include(d=>d.ArtPieces).First(s=>s.Name == "armazem");
            var pieceData = db.ArtPieces.Include(d=>d.RoomMuseum).Include(b=>b.ArtPieceState).First(d => d.Id == pieceId);  //Get data from one art piece by ID
            var getRoomId = pieceData.RoomMuseum.Id;
            var roomData = db.RoomMuseums.First(v => v.Id == getRoomId);  //Get data from one room by ID
            var eventId = roomData.Id;
            var eventData = db.Events.First(d => d.Id == eventId);  //Get data from one event by ID

            pieceData.ArtPieceState = getPieceStorageState;

            if (roomData.SumRoomArtPieces > 0)  // verify if number of pieces in the room is greater than 0
            {
                roomData.SumRoomArtPieces -= 1; // remove 1 art piece from 1 room

                if (eventData.SumArtPieces > 0)   // verify if number of pieces in all rooms (in the event) is greater than 0
                {
                    eventData.SumArtPieces -= 1;
                }
            }
            
            db.SaveChanges();
            
            return Redirect("ListArtPieces");;
        }

        public ActionResult SelectEventToAttachArtPice(string artpieceId)
        {
            var pieceId = Int32.Parse(artpieceId); // Convert artpieceId from string to int
            var getPieceDataFromId = db.ArtPieces.Include(d => d.ArtPieceState).Include(d => d.RoomMuseum).First(d => d.Id == pieceId);
            ViewBag.PieceSelected = getPieceDataFromId;


            var getExhibitionAcceptedState = db.EventStates.Include(d=>d.Events).Single(s => s.Name == "aceites");
            var ExhibitionAccepted = db.Events.Include(d=>d.EventType).Include(v=>v.UserAccount).Include(v=>v.OutSideSpaces).Include(d=>d.EventState).Where(d=>d.EventState.Name == getExhibitionAcceptedState.Name).ToList();
            ViewBag.ExhibitionsAccepted = ExhibitionAccepted;
            return View();
        }

        public ActionResult SelectRoomToAttachArtPiece(string artpieceID, string eventId)
        {
            var getPieceId = Int32.Parse(artpieceID); // Convert artpieceId from string to int
            var getPieceDataFromId = db.ArtPieces.Include(d=>d.RoomMuseum).Include(d=>d.ArtPieceState).First(d => d.Id == getPieceId);
            var getEventId = Int32.Parse(eventId); // Convert artpieceId from string to int
            var getSelectedEventData = db.Events.Include(d=>d.EventState).Include(d=>d.RoomMuseums).Include(d=>d.EventType).Include(d=>d.OutSideSpaces).Include(v=>v.UserAccount).First(d => d.Id == getEventId);
            var getRoomsEventSelected = db.RoomMuseums.Include(v=>v.ArtPieces).Include(v=>v.SpaceState).Where(d => d.Event.Id == getEventId).ToList();
            ViewBag.ExhibitionSelected = getSelectedEventData;
            ViewBag.PieceSelected = getPieceDataFromId;
            ViewBag.RoomsExhibition = getRoomsEventSelected;
            return View();
        }

        public ActionResult AttachArtPiece(string artpieceID, string eventId, string roomId)
        {
            var getPieceId = Int32.Parse(artpieceID); // Convert artpieceId from string to int
            var getPieceDataFromId = db.ArtPieces.Include(d => d.RoomMuseum).Include(d => d.ArtPieceState).First(d => d.Id == getPieceId);
            var getEventId = Int32.Parse(eventId); // Convert artpieceId from string to int
            var getSelectedEventData = db.Events.Include(d => d.EventState).Include(d => d.RoomMuseums).Include(d => d.EventType).Include(d => d.OutSideSpaces).Include(v => v.UserAccount).First(d => d.Id == getEventId);
            var getRoomtId = Int32.Parse(roomId); // Convert roomID from string to int
            var getSelectedRoomData = db.RoomMuseums.Include(d=>d.ArtPieces).Include(d=>d.SpaceState).First(d => d.Id == getRoomtId);

            var getPieceStorageState = db.ArtPieceStates.Include(d => d.ArtPieces).First(s => s.Name == "exposicao");  //Obter estado de peça em exibição

            //if (getSelectedRoomData.SumRoomArtPieces < getSelectedRoomData.MaxRoomArtPieces)
            //{
                getPieceDataFromId.ArtPieceState = getPieceStorageState;
                //var test = getPieceDataFromId.RoomMuseum.ArtPieces;
                //if (!test.Contains(getPieceDataFromId))
                //{
                    getPieceDataFromId.RoomMuseum.ArtPieces.Add(getPieceDataFromId);
                    getSelectedRoomData.SumRoomArtPieces += 1;  // Incrementar 1 peça à soma de peças na sala
                    getSelectedEventData.SumArtPieces += 1;  // Incrementar 1 peça à soma de peças no evento
                //}
            //}
              
            db.SaveChanges();

            return Redirect("ListArtPieces");
            
        }
    }
}
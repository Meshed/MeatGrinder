using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using MeatGrinder.Models;

namespace MeatGrinder.Controllers
{
    public class TestAPIController : ApiController
    {
        private MeatGrinderEntities db = new MeatGrinderEntities();

        // GET api/TestAPI
        public IEnumerable<Goal> GetGoals()
        {
            return db.Goals.AsEnumerable();
        }

        // GET api/TestAPI/5
        public Goal GetGoal(int id)
        {
            Goal goal = db.Goals.Find(id);
            if (goal == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return goal;
        }

        // PUT api/TestAPI/5
        public HttpResponseMessage PutGoal(int id, Goal goal)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != goal.ID)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(goal).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/TestAPI
        public HttpResponseMessage PostGoal(Goal goal)
        {
            if (ModelState.IsValid)
            {
                db.Goals.Add(goal);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, goal);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = goal.ID }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/TestAPI/5
        public HttpResponseMessage DeleteGoal(int id)
        {
            Goal goal = db.Goals.Find(id);
            if (goal == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Goals.Remove(goal);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, goal);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
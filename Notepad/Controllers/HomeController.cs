using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Notepad.Models;
using Notepad.Models.Query;


namespace Notepad.Controllers
{
    /**
     * General controller for single page applications.
     * REsponse for displaying and making operations over entities
     */
    public class HomeController : BaseController
    {

        /**
         * Main action, display entities.
         * Can be passed some options for managing displaying entities:
         *  page - number of viewing page
         *  count - entities on page
         *  order - direction of entitiens ordering
         *  by - field using for ordering
         * Returns HTML response.
         */
        public ActionResult List(int page = 0, int count = 3, string order = "DESC", string by = "Id")
        {
            page = Math.Abs(page);
            count = Math.Abs(count);
         
            Ordering ordering = new Ordering { Order = order, Field = by };

            this.ViewBag.Entries = this.repository.SelectEntries(page, count, null, ordering);
            this.ViewBag.HasMore = this.repository.SelectEntries(page + 1, count, null, ordering).Count > 0;
            
            return View();
        }

        /**
         * Action for getting entities list by AJAX request.
         * It's same with List, has some additional parameters:
         *  search - string for search
         *  field - field using for search
         * Returns JSON response.
         */
        public ActionResult AjaxList(int page = 0, int count = 3, string search = null, string field = null, string order = "DESC", string by = "Id")
        {
            page = Math.Abs(page);
            count = Math.Abs(count);

            Ordering ordering = new Ordering { Order = order, Field = by };
            ConditionCollection conditions = null;

            if (search != null)
            {
                conditions = new ConditionCollection();
                conditions.AddCondition(new LikeCondition { Field = field, Value = search });
            }

            var entries = this.repository.SelectEntries(page, count, conditions, ordering).Select(entry => new
            {
                Id = entry.Id,
                Name = entry.Name,
                Lastname = entry.Lastname,
                BirthYear = entry.BirthYear,
                PhoneNumber = entry.PhoneNumber
            });

            bool hasMore = this.repository.SelectEntries(page + 1, count, conditions, ordering).Count > 0;

            var response = new
            {
                list = entries,
                hasMore = hasMore
            };

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /**
         * Action for removing some entities. It removes entities with passed ids.
         */
        [HttpPost]
        public ActionResult AjaxRemove(int[] ids)
        {
            bool success = this.repository.DeleteEntries(ids);

            return Json(new { success = success });
        }

        /**
         * Method simply returns partial view of entity creation form.
         */
        public ActionResult Create()
        {
            this.ViewBag.Today = DateTime.Now;

            return View();
        }

        /**
         * Create new entity by passed parameters.
         * Here checks passed values from form and uniqueness provided phone number.
         * Returns JSON response with success flag.
         */
        [HttpPost]
        public ActionResult Create([Bind(Exclude = "Id")]NotepadEntry entry)
        {
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();

            if (!ModelState.IsValid)
            {
                for (int i = 0; i < ModelState.Keys.Count; i++)
                {
                    string key = ModelState.Keys.ElementAt(i);
                    if (!ModelState.IsValidField(key))
                    {
                        errors.Add(key, (from error in ModelState.Values.ElementAt(i).Errors select error.ErrorMessage).ToList<string>());
                    }
                }

                return Json(new { success = ModelState.IsValid, errors = errors });
            }
            else
            {
                ConditionCollection conditions = new ConditionCollection();
                conditions.AddCondition(new EqualCondition { Field = "PhoneNumber", Value = entry.PhoneNumber });

                if (this.repository.SelectEntries(0, 1, conditions, null).Count == 1)
                {
                    errors.Add("PhoneNumber", new List<string> { "Запись с таким номером уже существует" });

                    return Json(new { success = false, errors = errors });
                }
                else
                {
                    this.repository.Store(entry);

                    return Json(new { success = true, entry = entry });
                }

            }
        }

        /**
         * Action for editing existed entity.
         * Returns partial form with filled fields.
         */ 
        public ActionResult Edit(int id)
        {
            NotepadEntry notepad = this.repository.Find(id);

            if (notepad != null)
            {
                this.ViewBag.Data = notepad;
                this.ViewBag.Today = DateTime.Now;

                return View();
            }
            else
            {
                return RedirectToAction("Create", "MyController");
            }
        }

        /**
         * Save changes in edited entity. Return ersponse in JSON, with success flag.
         */ 
        [HttpPost]
        public ActionResult Edit([Bind]NotepadEntry entry)
        {
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();

            if (!ModelState.IsValid)
            {
                for (int i = 0; i < ModelState.Keys.Count; i++)
                {
                    string key = ModelState.Keys.ElementAt(i);
                    if (!ModelState.IsValidField(key))
                    {
                        errors.Add(key, (from error in ModelState.Values.ElementAt(i).Errors select error.ErrorMessage).ToList<string>());
                    }
                }
            }
            else
            {
                NotepadEntry original = this.repository.Find(entry.Id);

                if (original != null)
                {
                    ConditionCollection conditions = new ConditionCollection();
                    conditions.AddCondition(new EqualCondition { Field = "PhoneNumber", Value = entry.PhoneNumber });

                    if (original.PhoneNumber != entry.PhoneNumber && this.repository.SelectEntries(0, 1, conditions, null).Count == 1)
                    {
                        errors.Add("PhoneNumber", new List<string> { "Записи таким номером уже существует" });
                    }
                    else {

                        this.repository.Update(entry);
                        return Json(new { success = true, entry = entry });
                    }
                }
                else {
                    errors.Add("PhoneNumber", new List<string> { "Записи с таким номером не существует" });
                }

            }

            return Json(new { success = false, errors = errors });
        }

        /**
         * Action for saving current entities to storage.
         */ 
        public ActionResult Save()
        {
            this.repository.Dispose();
            return Json(new { success = true}, JsonRequestBehavior.AllowGet);
        }

        
    }
}
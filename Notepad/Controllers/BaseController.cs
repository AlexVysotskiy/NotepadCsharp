using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Notepad.Models.Repository;
using Notepad.Models;

namespace Notepad.Controllers
{
    /**
     * Base controller, contains general methods, fields, resource and etc.
     */
    public class BaseController : Controller
    {
        /**
         * Source of the entities to display
         */
        protected IRepository<NotepadEntry> repository;

        public BaseController()
        {
            this.repository = XMLRepository.Instance;
        }
    }
}
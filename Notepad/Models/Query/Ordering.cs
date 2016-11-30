using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Notepad.Models.Query
{
    /**
     * Class keep information about field and direction of sorting 
     */ 
    public class Ordering
    {
        public const string DESC = "DESC";
        public const string ASC = "ASC";

        protected string order;
        protected string field;

        public Ordering()
        {
        }

        public Ordering(string order, string field)
        {
            this.Order = order;
            this.Field = field;
        }

        public string Order
        {
            get
            {
                return this.order;
            }
            set
            {

                if (value == Ordering.ASC || value == Ordering.DESC)
                {
                    this.order = value;
                }
                else
                {
                    this.order = Ordering.ASC;
                }
            }
        }

        public string Field
        {
            get {
                return this.field;
            } set {
                this.field = value;
            }
        }
    }

}
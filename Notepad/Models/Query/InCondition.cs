using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Notepad.Models.Query
{
    /**
     * Simple condition, check is object property equals to estimated value
     */ 
    public class InCondition : Condition
    {
        public InCondition()
        {
            this.Type = Condition.TypeIn;
        }

        public override bool Check(object obj)
        {
            var field = this.GetProperty(obj);
            
            if (field != null)
            {
                var search = field.GetValue(obj);
                foreach (var val in this.value)
                {
                    if (search.Equals(val))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

    }
}
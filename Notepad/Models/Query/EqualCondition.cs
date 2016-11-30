using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Notepad.Models.Query
{
    /**
     * Simple condition, check is object property equals to estimated value
     */ 
    public class EqualCondition : Condition
    {
        public EqualCondition()
        {
            this.Type = Condition.TypeEqual;
        }

        public override bool Check(object obj)
        {
            var field = this.GetProperty(obj);

            if (field != null)
            {
                return field.GetValue(obj).Equals(this.value);
            }

            return false;
        }

    }
}
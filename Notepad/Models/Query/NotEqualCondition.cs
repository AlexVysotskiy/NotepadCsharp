using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Notepad.Models.Query
{
    /**
    * Simple condition, check is object property not equals to estimated value
    */
    public class NotEqualCondition : Condition
    {
        public NotEqualCondition()
        {
            this.Type = Condition.TypeNotEqual;
        }

        public override bool Check(object obj)
        {
            var field = this.GetProperty(obj);

            if (field != null)
            {
                return !field.GetValue(obj).Equals(this.value);
            }

            return false;
        }
    }
}
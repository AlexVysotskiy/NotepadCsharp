using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Notepad.Models.Query
{
    /**
     * Check, is object property start with passed value
     */
    public class LikeCondition : Condition
    {
        public LikeCondition()
        {
            this.Type = Condition.TypeLike;
        }

        public override bool Check(object obj)
        {
            var field = this.GetProperty(obj);

            if (field != null)
            {
                return field.GetValue(obj).ToString().StartsWith(this.value.ToString());
            }

            return false;
        }
    }
}
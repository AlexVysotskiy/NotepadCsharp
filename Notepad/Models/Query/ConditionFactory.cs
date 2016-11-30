using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Notepad.Models.Query
{
    /**
     * Factrory for creating conditions by requested type
     */ 
    public class ConditionFactory
    {
        public static Condition GetCondition(string type)
        {
            switch (type)
            {
                case Condition.TypeEqual:
                    {
                        return new EqualCondition();
                    }
                case Condition.TypeNotEqual:
                    {
                        return new NotEqualCondition();
                    }
                case Condition.TypeLike:
                    {
                        return new LikeCondition(); ;
                    }
                case Condition.TypeIn:
                    {
                        return new InCondition(); ;
                    }
            }

            return null;
        }

    }
}
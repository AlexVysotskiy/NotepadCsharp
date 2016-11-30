using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Notepad.Models.Query
{
    /**
     * Collection of all conditions for one request.
     * It allows to add new conditions or remove all.
     * Also, it has CheckConditions method, which invokes all stored condition.
     */
    public class ConditionCollection
    {
        protected List<Condition> conditions = new List<Condition>();

        public ConditionCollection()
        {
        }

        public List<Condition> Conditions
        {
            get
            {
                return this.conditions;
            }
            set
            {
                this.conditions.AddRange(value);
            }
        }

        public void AddCondition(Condition condition)
        {
            this.conditions.Add(condition);
        }

        public void Clear()
        {
            this.conditions.Clear();
        }

        /**
         * Method define, is passed object satisfy all saved conditions.
         */
        public bool CheckConditions(object obj)
        {
            
            for (int i = 0; i < this.conditions.Count; i++)
            {
                if (!this.conditions.ElementAt(i).Check(obj))
                {
                    return false;
                }
            }

            return true;
        }

    }
}
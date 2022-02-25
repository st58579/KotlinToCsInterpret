using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KotlinToCs_Hrychanok.Interpreting
{
    class SymbolTable
    {
        private SymbolTable parent;
        private Dictionary<string, object> doubleVariables;
        private Dictionary<string, object> stringVariables;
        private Dictionary<string, object> dynamicalyTypedVariables;

        public SymbolTable(SymbolTable parent = null)
        {
            dynamicalyTypedVariables = new Dictionary<string, object>();
            stringVariables = new Dictionary<string, object>();
            doubleVariables = new Dictionary<string, object>();
            this.parent = parent;
        }

        public object Get(string name)
        {
            object value = null;
            if (dynamicalyTypedVariables.ContainsKey(name))
                value = dynamicalyTypedVariables[name];
            else if (doubleVariables.ContainsKey(name))
                value = doubleVariables[name];
            else if (stringVariables.ContainsKey(name))
                value = stringVariables[name];
            else if(parent != null)
                return parent.Get(name);
            return value;
        }

        public void SetString(string name, object value, out string error)
        {
            error = "";
            if (doubleVariables.ContainsKey(name))
            {
                error = "Variable with name " + name + " is already defined as Double";
                return;
            }
            if (stringVariables.ContainsKey(name))
                stringVariables[name] = value;
            else if (dynamicalyTypedVariables.ContainsKey(name))
                dynamicalyTypedVariables[name] = value;
            else if (parent != null)
            {
                parent.SetString(name, value, out string err);
                error = err;
                if(error != "")
                {
                    error += "\nAssigning value" + value + " to variable with name <" + name + "> is incorrect. " + name + " does not exist in current context.";
                    return;
                }
            }
        }

        public void SetDouble(string name, object value, out string error)
        {
            error = "";
            if (stringVariables.ContainsKey(name))
            {
                error = "Variable with name " + name + " is already defined as Double";
                return;
            }
            if (doubleVariables.ContainsKey(name))
                doubleVariables[name] = value;
            else if (dynamicalyTypedVariables.ContainsKey(name))
                dynamicalyTypedVariables[name] = value;
            else if (parent != null)
            {
                parent.SetDouble(name, value, out string err);
                error = err;
                if (error != "")
                {
                    error += "\nAssigning value" + value + " to variable with name <" + name + "> is incorrect. " + name + " does not exist in current context.";
                    return;
                }
            }
        }

        public object GetDynamic(string name)
        {
            var value = dynamicalyTypedVariables[name];
            if(value == null && parent != null)
            {
                return parent.GetDynamic(name);
            }
            return value;
        }

        public void SetDynamic(string name, object value)
        {
            dynamicalyTypedVariables[name] = value;
        }


        public object GetStringVar(string name)
        {
            if (stringVariables.ContainsKey(name))
            {
                var value = stringVariables[name];
                if (value == null && parent != null)
                {
                    return parent.GetStringVar(name);
                }
                return value.ToString();
            }
            return null;
        }

        public void SetStringVar(string name, string value)
        {
            stringVariables[name] = value;
        }

        public object GetDoubleVar(string name)
        {
            if (doubleVariables.ContainsKey(name))
            {
                var value = doubleVariables[name];
                if (value == null && parent != null)
                {
                    return parent.GetDynamic(name);
                }
                return (double)value;
            }
            return null;
        }

        public void SetDoubleVar(string name, double value)
        {
            doubleVariables[name] = value;
        }

        public void Remove(string name)
        {
            dynamicalyTypedVariables.Remove(name);
        }
    }
}

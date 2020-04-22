using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZP.Lib.Main
{
    public static class ZFSMConditions
    {
        public static void Build(object obj, IZFSM fsm)
        {
            //init action
            MethodInfo[] methodInfos = obj.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var m in methodInfos)
            {
                var a = m.GetCustomAttribute<FSMConditionAttribute>();
                if (a == null)
                    continue;


                fsm.AddCondition(a.ConName,ev =>
                {
                    return (bool)m.Invoke(obj, new object[] { ev});
                });
            }
        }
    }
}

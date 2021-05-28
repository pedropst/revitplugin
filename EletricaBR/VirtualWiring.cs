using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEletrica
{
    public class VirtualWiring
    {
        public VirtualConduit parentConduit;
        public String circuitString;
        public String parameterName;
        public List<WiringType> l_wireType = new List<WiringType>();

        public VirtualWiring(VirtualConduit parentConduit)
        {
            this.parentConduit = parentConduit;
            this.circuitString = circuitString;
        }

        public void CreatingWireType()
        {
            foreach (String s in parentConduit.circuits)
            {

            }
        }


        public void AssemblingParameterName()
        {
            if (this.circuitString.Contains("FF"))
            {

            }
            else if (this.circuitString.Contains("FN"))
            { 
            
            }
            else if (this.circuitString.Contains("FR"))
            {


            }
            else if (this.circuitString.Contains("NR"))
            {

            }
            else if (this.circuitString.Contains("FN"))
            {

            }
            else if (this.circuitString.Contains("FN"))
            {

            }
            else if (this.circuitString.Contains("FN"))
            {

            }


        }

    }
}

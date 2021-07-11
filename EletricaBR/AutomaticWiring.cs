using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace TCC
{
    class AutomaticWiring : IExternalCommand
    {
        #region <-- INSTRUÇÕES -->
        /*
        INSTRUÇÕES
        - Determinar o caminho de cada circuito 
            - CLASSE SEPARADA
        - Determinar a posição das representações de fiação
        - Inserir as fiações
        - Armazenar os dados: eletrodutos virtuais e circuitos presentes nos mesmos
            - ESTUDAR OS CAMPOS SERIALIZABLES

        */
        #endregion
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
        }
        #region
        #endregion
    }
}

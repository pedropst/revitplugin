//CHAMANDO AS BIBLIOTECAS DEFAULT, NORMALMENTE SEMPRE QUE CRIAR UMA CLASSE NOVA JÁ IRÁ APARECER COM ESSAS BIBLIOTECAS AI
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//CHAMANDO A BIBLIOTECA DO REVIT
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

//CHAMANDO A BIBLIOTECA QUE PERMITE CRIAR AS JANELAS QUE PERGUNTAM AO USUÁRIO ALGUMA COISA, POR EXEMPLO, OS NOMES DOS PARÂMETROS E O NOME DA FAMÍLIA
using Microsoft.VisualBasic;


namespace EasyEletrica
{
    //SEMPRE IRÁ ESCREVER ESSAS DUAS PRÓXIMAS LINHAS SE ESTIVER CRIANDO UM ARQUIVO QUE IRÁ INTERAGIR COM O REVIT
    [TransactionAttribute(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class HerdandoParametros : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //PROVAVELMENTE PRIMEIRAS LINHAS DE QUALQUER PLUGIN QUE VOCÊ VAI FAZER
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;


            //CRIANDO A JANELA ONDE PEDE QUAIS SÃO OS PARÂMETROS
            String paramText = Interaction.InputBox("Insira os parâmetros (separados por ';'):", "Pedro Elétrica","PL-AMBIENTE;PL- PAVIMENTO;PL-APARTAMENTO;PL-SETOR;PL-TIPO DE INSTALAÇÕES", -1, -1);
            //CRIANDO UMA LISTA FORMADA PELO TEXTO RESPONDIDO NA JANELA, SENDO QUE O SIMBOLO QUE DENOTA A SEPAÇÃO É O ;
            List<String> paramNames = paramText.Split(';').ToList(); 


            //CRIANDO OUTRA JANELA QUE PERGUNTA QUAL É O NOME DA FAMÍLIA, E ARMAZENANDO ISSO NA VARIÁVEL familyName
            String familyName = Interaction.InputBox("Insira o nome da família:", "Pedro Elétrica", "", -1, -1);


            //UM FILTRO QUE PEGA TODOS OS ELEMENTOS QUE ESTÃO NO DOCUMENTO
            FilteredElementCollector allElements = new FilteredElementCollector(doc);
            //UTILIZO UM MÉTODO DO FILTRO QUE PERMITE EU FILTRAR PELO TIPO DE CLASSE, FamilyInstance É A CLASSE DOS ELEMENTOS DISPOSTOS DO DOCUMENTO
            allElements.OfClass(typeof(FamilyInstance));


            //CRIO DUAS LISTAS DE ELEMENTOS UMA QUE SERÁ COMPOSTA PELO ELEMENTO ANINHADO (SUBELEMENT) E OUTRA PARA O ELEMENTO PAI (SUPERCOMPONENT)
            List<Element> subComponents = new List<Element>();
            List<Element> superComponents = new List<Element>();
            /*VERIFICO ELEMENTO POR ELEMENTO CONTIDO NAQUELE FILTRO DA LINHA 38, E VERIFICO SE O NOME DA FAMÍLIA DESSE ELEMENT É O NOME INSERIDO NA LINHA 32
            SE ISSO FOR VERDADE IREI ADICIONAR A LISTA subComponents ESSE ELEMENTO, E TAMBÉM IREI ADICIONAR O SUPERCOMPONENTE DESSE ELEMENT NA LISTA superComponents
             */
            foreach (Element e in allElements)
            {
                if (e.Name == familyName)
                {
                    subComponents.Add(e);
                    superComponents.Add(doc.GetElement((e as FamilyInstance).SuperComponent.Id));
                }
            }


            //INDICO AO REVIT QUE IREI FAZER MUDANÇAS NO DOCUMENTO
            using (Transaction trans = new Transaction(doc, "HERDANDO PARÂMETROS"))
            {
                //INDICO QUE IREI COMEÇAR
                trans.Start();

                //CHAMO O MÉTODO/FUNÇÃO QUE ESTÁ DEFINIDA LOGO ABAIXO - DAVA PARA TER FEITO DIRETO AQUI, BOBEIRA D+ FAZER UMA FUNÇÃO SÓ PARA ISSO KKKKKKK
                Rodando(subComponents, superComponents, paramNames);

                //INDICO QUE FINALIZEI
                trans.Commit();
            }

            return Result.Succeeded;    
        }

        //FUNÇÃO QUE RECEBE UMA LISTA COM OS SUBCOMPONENTS, UMA LISTA COM SUPERCOMPONENTS, E UMA LISTA COM OS PARÂMETROS
        public void Rodando(List<Element> e_child, List<Element> e_parent, List<String> paramNames)
        {
            //VERIFICO PARA CADA ELEMENTO DA LIST SUBCOMPONENTS (FUNCIONARIA DO MESMO JEITO SE FOSSE A LISTA SUPERCOMPONENTS, VISTO QUE AMBAS IRÃO TER A MESMA QUANTIDADE DE ELEMENTOS)
            for(int i = 0; i < e_parent.Count; i++)
            {
                //PARA CADA NOME DE PARÂMETRO IREI ...
                for (int j = 0; j < paramNames.Count; j++)
                {
                    //... SELECIONAR O VALOR DESSE PARÂMETRO NO ELEMENTO SUPERCOMPONENT;
                    String paramValue = e_parent[i].LookupParameter(paramNames[j]).AsString();
                    //... SELECIONAR O PARÂMETRO CONTIDO NO ELEMENTO DO SUBCOMPONENT;
                    Parameter param = e_child[i].LookupParameter(paramNames[j]);
                    //... DEFINIR O VALOR ENCONTRADO NO SUPERCOMPONENT AO ELEMENTO DO SUBCOMPONENT
                    param.Set(paramValue);

                }
            }

        }

    }
}

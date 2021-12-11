﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Curriculum.Models
{
    public class Itens
    {
        //lista de estados para select
        private SelectListItem[] estados = new SelectListItem[]
        {
            new SelectListItem { Value = "", Text = "--" },
            new SelectListItem { Value = "AL", Text = "AL" },
            new SelectListItem { Value = "AP", Text = "AP" },
            new SelectListItem { Value = "AM", Text = "AM" },
            new SelectListItem { Value = "BA", Text = "BA" },
            new SelectListItem { Value = "CE", Text = "CE" },
            new SelectListItem { Value = "DF", Text = "DF" },
            new SelectListItem { Value = "ES", Text = "ES" },
            new SelectListItem { Value = "GO", Text = "GO" },
            new SelectListItem { Value = "MA", Text = "MA" },
            new SelectListItem { Value = "MT", Text = "MT" },
            new SelectListItem { Value = "MS", Text = "MS" },
            new SelectListItem { Value = "MG", Text = "MG" },
            new SelectListItem { Value = "PA", Text = "PA" },
            new SelectListItem { Value = "PB", Text = "PB" },
            new SelectListItem { Value = "PR", Text = "PR" },
            new SelectListItem { Value = "PE", Text = "PE" },
            new SelectListItem { Value = "PI", Text = "PI" },
            new SelectListItem { Value = "RJ", Text = "RJ" },
            new SelectListItem { Value = "RN", Text = "RN" },
            new SelectListItem { Value = "RS", Text = "RS" },
            new SelectListItem { Value = "RO", Text = "RO" },
            new SelectListItem { Value = "RR", Text = "RR" },
            new SelectListItem { Value = "SC", Text = "SC" },
            new SelectListItem { Value = "SP", Text = "SP" },
            new SelectListItem { Value = "SE", Text = "SE" },
            new SelectListItem { Value = "TO", Text = "TO" },
        };

        //método para criar lista de cursos para select
        public SelectListItem[] cursos(List<Curso> listaCursos)
        {
            //criação da lista de itens do select e adição do valor padrão
            List<SelectListItem> lista = new List<SelectListItem>();
            lista.Add(new SelectListItem { Value = "", Text = "--" });

            //se houver uma lista de cursos válida,
            if (listaCursos != null && listaCursos.Count > 0)
            {
                //para cada curso na lista,
                foreach (Curso curso in listaCursos)
                {
                    //criado um item e adicionado à lista
                    SelectListItem item = new SelectListItem { Value = curso.AbrevCurso, Text = string.Format("{0} ({1})", curso.NomeCurso, curso.AbrevCurso) };
                    lista.Add(item);
                }
            }
            //retorna a lista de itens em formato de array
            return lista.ToArray();
        }

        //propriedade para obter a lista de estados
        public SelectListItem[] Estados { get => estados; }
    }
}
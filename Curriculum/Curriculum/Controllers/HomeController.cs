﻿using Curriculum.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Curriculum.Controllers
{
    public class HomeController : Controller
    {
        //retorna a view 'Index'
        public ActionResult Index()
        {
            return View();
        }
        //retorna a view 'CadastrarAluno'
        public ActionResult CadastrarAluno()
        {
            //cria uma lista com os cursos disponíveis para cadastrar e passa para a view
            List<Curso> lista = Curso.listarCursos();
            return View(lista);
        }
        //retorna a view 'Conta'
        public ActionResult Conta()
        {
            //obtém um usuário da sessão
            Usuario usuario = (Usuario)Session["Usuario"];

            //se o usuário não for nulo e possuir código de aluno,
            if (usuario != null && usuario.CodAluno != null)
            {
                //obtém dados do aluno, curso feito, currículo e vagas nas quais se candidatou
                Aluno aluno = new Aluno().buscarDados(usuario.CodAluno);
                Curso curso = Curso.buscarCursoFeito(usuario.CodAluno);
                Curriculo curriculo = new Curriculo().buscarCurriculo(usuario.CodAluno);
                List<Vaga> lista = Vaga.listarVagasCandidatadas(usuario.CodAluno);
                //passa todos os dados como TempData
                TempData["vagasCandidatadas"] = lista;
                TempData["aluno"] = aluno;
                TempData["cursoFeito"] = curso;
                TempData["curriculo"] = curriculo;
            }
            return View();
        }
        //retorna a view 'EditarDados'
        public ActionResult EditarDados()
        {
            //obtém um usuário da sessão
            Usuario u = (Usuario)Session["Usuario"];

            //se o usuário não for nulo e possuir código de aluno,
            if (u != null && u.CodAluno != null)
            {
                //obtém dados do aluno, curso feito, currículo e vagas nas quais se candidatou
                Aluno aluno = new Aluno().buscarDados(u.CodAluno);
                Curso curso = Curso.buscarCursoFeito(u.CodAluno);
                Curriculo curriculo = new Curriculo().buscarCurriculo(u.CodAluno);
                List<Curso> lista = Curso.listarCursos();
                //passa todos os dados como TempData
                TempData["aluno"] = aluno;
                TempData["cursoFeito"] = curso;
                TempData["curriculo"] = curriculo;
                TempData["cursos"] = lista;
            }
            return View();
        }
        //retorna a view 'CandidatarVagas'
        public ActionResult CandidatarVagas()
        {
            //obtém um usuário da sessão
            Usuario usuario = (Usuario)Session["Usuario"];

            //se o usuário não for nulo e possuir código de aluno,
            if (usuario != null && usuario.CodAluno != null)
            {
                //obtém dados do aluno e curso feito e passa como TempData
                Aluno aluno = new Aluno().buscarDados(usuario.CodAluno);
                Curso curso = Curso.buscarCursoFeito(usuario.CodAluno);
                TempData["aluno"] = aluno;

                //se o aluno for empregável,
                if (aluno.Empregavel)
                {
                    //obtém uma lista de todas as vagas relacionadas ao curso feito pelo aluno
                    List<Vaga> lista = Vaga.buscarVagasCurso(curso.AbrevCurso);
                    //a lista é passada como TempData
                    TempData["listaVagas"] = lista;
                }
            } 
            return View();
        }
        //retorna a view 'PesquisarAlunos'
        public ActionResult PesquisarAlunos()
        {
            //obtém uma lista com todos os cursos cadastrados para pesquisa e passa para a view
            List<Curso> lista = Curso.listarCursos();
            return View(lista);
        }
        //retorna a view 'CadastrarCurso'
        public ActionResult CadastrarCurso()
        {
            return View();
        }
        //retorna a view 'DadosAluno'
        public ActionResult DadosAluno()
        {
            return View();
        }
        //retorna a view 'CadastrarVaga'
        public ActionResult CadastrarVaga()
        {
            //obtém uma lista com todos os cursos cadastrados para cadastro da vaga e passa para a view
            List<Curso> lista = Curso.listarCursos();
            return View(lista);
        }
        //retorna a view 'DadosVaga'
        public ActionResult DadosVaga()
        {
            return View();
        }
        //retorna a view 'EditarVaga'
        public ActionResult EditarVaga(string id)
        {
            //obtém lista de cursos cadastrados
            List<Curso> lista = Curso.listarCursos();
            //cria uma vaga usando o código informado
            Vaga vaga = new Vaga().buscarDados(id);
            //passa os dados como TempData
            TempData["vaga"] = vaga;
            TempData["cursos"] = lista;
            return View();
        }

        //método HttpPost da view 'CadastrarAluno'
        [HttpPost]
        public ActionResult CadastrarAluno(string nome, string sobrenome, string bairro, string cidade,
            string cursoRealizado, string dataNascimento, string telefone, string cep, string estado, HttpPostedFileBase arquivo,
            string palavrasChave, string nomeUsuario, string senha, string confirmacao)
        {
            //a mensagem é nula
            string msg = null;

            //validação de todos os dados recebidos
            nome = Verificacao.validarTexto(nome, 1, 20);
            sobrenome = Verificacao.validarTexto(sobrenome, 1, 20);
            telefone = Verificacao.validarMascara(telefone, 14);
            bairro = Verificacao.validarTexto(bairro, 1, 30);
            cidade = Verificacao.validarTexto(cidade, 1, 30);
            estado = Verificacao.validarAbreviacao(estado, 2, 2);
            cep = Verificacao.validarMascara(cep, 9);
            palavrasChave = Verificacao.validarPalavrasChave(palavrasChave, 0, 180);
            cursoRealizado = Verificacao.validarAbreviacao(cursoRealizado, 2, 5);
            nomeUsuario = Verificacao.validarNomeUsuario(nomeUsuario);
            senha = Verificacao.validarSenha(senha, confirmacao);

            //se nenhum dos dados for nulo e o arquivo for .pdf ou .docx,
            if (nome != null && sobrenome != null && telefone != null && bairro != null && cidade != null
                && estado != null && cep != null && cursoRealizado != null && arquivo != null && arquivo.ContentLength > 0
                && (arquivo.FileName.EndsWith(".pdf") || arquivo.FileName.EndsWith(".docx") && nomeUsuario != null && senha != null))
            {
                //o nome completo passa a ser o nome e sobrenome juntos
                string nomeCompleto = nome + " " + sobrenome;
                //criação do aluno
                Aluno aluno = new Aluno(nomeCompleto, bairro, cidade, estado, cep, DateTime.Parse(dataNascimento), telefone, palavrasChave, true);
                //criação do usuário
                Usuario usuario = new Usuario(nomeUsuario, senha, null, false);

                //se o nome de usuário estiver disponível,
                if (usuario.checarNomeUsuario())
                {
                    //se o usuário for cadastrado,
                    if (usuario.inserirUsuario())
                    {
                        //se o aluno for cadastrado,
                        if (aluno.inserirAluno())
                        {
                            //o código do aluno é obtido
                            string codAluno = aluno.codAlunoTemp.ToString();
                            //o currículo é criado
                            Curriculo curriculo = new Curriculo(codAluno, arquivo);

                            //se o curso feito e currículo forem cadastrados,
                            if (Curso.inserirCursoFeito(codAluno, cursoRealizado) && curriculo.inserirCurriculo())
                            {
                                //se o usuário e alunos forem ligados,
                                if (usuario.ligarAluno(codAluno))
                                {
                                    //mensagem de sucesso
                                    msg = "Cadastrado com sucesso";
                                }
                                else
                                {
                                    //caso contrário, é exibida uma mensagem de erro
                                    msg = "Um erro ocorreu ao cadastrar. Tente novamente";
                                }
                            }
                            else
                            {
                                //caso contrário, o usuário cadastrado é excluído
                                usuario.apagarUsuario();
                            }
                        }
                        else
                        {
                            //caso contrário, o usuário cadastrado é excluído
                            usuario.apagarUsuario();
                        }
                    }
                    else
                    {
                        //caso contrário, mostra uma mensagem de erro
                        msg = "Um erro ocorreu ao cadastrar. Tente novamente";
                    }
                }
                else
                {
                    //caso conrário, informa que o nome de usuário não está disponível
                    msg = string.Format("O nome de usuário inserido ({0}) já está sendo utilizado", nomeUsuario);
                }
            }
            else
            {
                //caso os dados sejam nulos, retorna uma mensagem avisando para 
                //preencher os campos corretamente
                msg = "Preencha os campos corretamente";
            }
            //passa a mensagem como TempData
            TempData["msg"] = msg;
            //retorna para a view 'CadastrarAluno'
            return RedirectToAction("CadastrarAluno");
        }

        //método HttpPost da view 'EditarDados'
        [HttpPost]
        public ActionResult EditarDados(string nomeCompleto, string bairro, string cidade,
            string cursoRealizado, string dataNascimento, string telefone, string cep, string estado, HttpPostedFileBase arquivo,
            string palavrasChave, string nomeUsuario, string senha, string confirmacao)
        {
            //mensagem padrão para casos de erro
            string msg = "Nenhum dado foi atualizado. Tente novamente";
            //obtém um usuário da sessão
            Usuario usuario = (Usuario)Session["Usuario"];

            //se o usuário não for nulo,
            if (usuario != null)
            {
                //valida a senha nova informada para alteração
                senha = Verificacao.validarSenha(senha, confirmacao);

                //se o usuário possuir código de aluno,
                if (usuario.CodAluno != null)
                {
                    //obtém dados do aluno, curso feito e currículo
                    Aluno aluno = new Aluno().buscarDados(usuario.CodAluno);
                    Curso cursoFeito = Curso.buscarCursoFeito(usuario.CodAluno);
                    Curriculo curriculo = new Curriculo().buscarCurriculo(usuario.CodAluno);

                    //validação dos campos para atualização
                    nomeCompleto = Verificacao.validarTexto(nomeCompleto, 1, 40);
                    telefone = Verificacao.validarMascara(telefone, 14);
                    bairro = Verificacao.validarTexto(bairro, 1, 30);
                    cidade = Verificacao.validarTexto(cidade, 1, 30);
                    estado = Verificacao.validarAbreviacao(estado, 2, 2);
                    cep = Verificacao.validarMascara(cep, 9);
                    cursoRealizado = Verificacao.validarAbreviacao(cursoRealizado, 2, 5);
                    palavrasChave = Verificacao.validarPalavrasChave(palavrasChave, 0, 180);

                    //se os dados do aluno não forem nulos,
                    if (nomeCompleto != null && telefone != null && bairro != null && cidade != null
                        && estado != null && cep != null)
                    {
                        //cria um aluno com os dados novos
                        Aluno alunoNovo = new Aluno(aluno.CodAluno, nomeCompleto, bairro, cidade, estado, cep, DateTime.Parse(dataNascimento), telefone, palavrasChave, aluno.Empregavel);

                        //se houver sucesso na atualização do aluno,
                        if(alunoNovo.atualizarAluno(aluno, alunoNovo))
                        {
                            //a mensagem passa a ser de sucesso
                            msg = "Dados atualizados";
                        }
                    }

                    //se o curso realizado não for nulo,
                    if (cursoRealizado != null)
                    {
                        //se não houver um curso feito,
                        if (cursoFeito == null)
                        {
                            //se houver sucesso em inserir o curso feito
                            if (Curso.inserirCursoFeito(aluno.CodAluno, cursoRealizado))
                            {
                                //passa mensagem de sucesso
                                msg = "Dados atualizados";
                            }
                        }
                        else
                        {
                            //caso contrário, se houver sucesso em atualizar o curso feito,
                            if (Curso.atualizarCursoFeito(cursoFeito.AbrevCurso, cursoRealizado, aluno.CodAluno))
                            {
                                //passa mensagem de sucesso
                                msg = "Dados atualizados";
                            }
                        }
                    }

                    //se o arquivo não for nulo, possuir tamanho maior que zero e for .pdf ou .docx,
                    if (arquivo != null && arquivo.ContentLength > 0 && (arquivo.FileName.EndsWith(".pdf") || arquivo.FileName.EndsWith(".docx")))
                    {
                        //cria um currículo com dados novos
                        Curriculo curriculoNovo = new Curriculo(aluno.CodAluno, arquivo);

                        //se não houver currículo cadastrado,
                        if (curriculo == null)
                        {
                            //se houver sucesso na inserção do currículo,
                            if (curriculoNovo.inserirCurriculo())
                            {
                                //altera o estado do aluno para empregável e passa mensagem de sucesso
                                Aluno.mudarEmpregavel(aluno.CodAluno, true);
                                msg = "Dados atualizados";
                            }
                            else
                            {
                                //em caso de erro, o estado do aluno é não empregável
                                Aluno.mudarEmpregavel(aluno.CodAluno, false);
                            }
                        }
                        else
                        {
                            //caso haja um currículo, ele é atualizado e é passado uma mensagem de sucesso
                            curriculoNovo.atualizarCurriculo(curriculo, curriculoNovo);
                            msg = "Dados atualizados";
                        }
                    }
                }

                //se a senha não for nula,
                if (senha != null)
                {
                    //se houver sucesso na atualização da senha do usuário,
                    if (usuario.atualizarSenha(usuario.Senha, senha, usuario.NomeUsuario))
                    {
                        //desconecta o usuário
                        return Desconectar();
                    } 
                }
            }
            //passa a mensagem como TempData
            TempData["msg"] = msg;
            //retorna a view 'Conta'
            return RedirectToAction("Conta");
        }     
        //método HttpPost da view 'Conta'
        [HttpPost]
        public ActionResult Conta(string nomeUsuario, string senha)
        {
            //autentição do usuário
            Usuario u = Usuario.Autenticar(nomeUsuario, senha);
            //a mensagem é nula
            string msg = null;

            //se o usuário for nulo (a autenticação falhou),
            if (u == null)
            {
                //passa uma mensagem de erro
                msg = "Usuário ou senha incorretos";
            }
            //caso o usuário não seja nulo (a autenticaçaõ foi bem sucedida),
            else if (u != null)
            {
                //adiciona o usuário a uma sessão
                Session.Add("Usuario", u);
                //passa uma mensagem de sucesso
                msg = "Bem-vindo(a), " + u.NomeUsuario;
            }
            //adicionada a mensagem como TempData
            TempData["msg"] = msg;
            //retorna a view 'Conta'
            return RedirectToAction("Conta");
        }
        //método HttpPost da view 'CadastrarCurso'
        [HttpPost]
        public ActionResult CadastrarCurso(string nomeCurso, string abrevCurso)
        {
            //a mensagem é nula
            string msg = null;
            //validação do nome e abreviação do curso
            nomeCurso = Verificacao.validarTexto(nomeCurso, 1, 34);
            abrevCurso = Verificacao.validarAbreviacao(abrevCurso, 2, 5);

            //se os dados não forem nulos,
            if (nomeCurso != null && abrevCurso != null)
            {
                //criado um curso
                Curso curso = new Curso(nomeCurso, abrevCurso);

                //em caso de sucesso na inserção do curso,
                if (curso.inserirCurso())
                {
                    //passa mensagem de sucesso
                    msg = "Curso cadastrado com sucesso";
                }
                else
                {
                    //em caso de erro, passa mensagem de erro
                    msg = "Não foi possível cadastrar o curso. Tente novamente";
                }
            }
            //adiciona a mensagem como TempData
            TempData["msg"] = msg;
            //retorna a view 'CadastrarCurso'
            return RedirectToAction("CadastrarCurso");
        }
        //método HttpPost da view 'CadastrarCurso'
        [HttpPost]
        public ActionResult CadastrarVaga(string nomeEmpresa, string nomeVaga, string cidade, string estado, string descricao, string cursoRelacionado)
        {
            //mensagem nula
            string msg = null;
            //validação dos dados informados
            nomeEmpresa = Verificacao.validarTextoCompleto(nomeEmpresa, 1, 20);
            nomeVaga = Verificacao.validarTexto(nomeVaga, 1, 30);
            cidade = Verificacao.validarTexto(cidade, 1, 30);
            estado = Verificacao.validarTexto(estado, 2, 2);
            descricao = Verificacao.validarTextoCompleto(descricao, 1, 280);
            cursoRelacionado = Verificacao.validarAbreviacao(cursoRelacionado, 2, 5);

            //se os dados não forem nulos,
            if (nomeEmpresa != null && nomeVaga != null && cidade != null && estado != null && descricao != null && cursoRelacionado != null)
            {
                //criado uma vaga
                Vaga vaga = new Vaga(nomeEmpresa, nomeVaga, cidade, estado, descricao, cursoRelacionado);

                //se houver sucesso na inserção da vaga,
                if (vaga.inserirVaga())
                {
                    //passada mensagem de sucesso
                    msg = "Vaga cadastrada com sucesso";
                }
                else
                {
                    //em caso de erro, passada mensagem de erro
                    msg = "Erro ao cadastrar a vaga. Tente novamente";
                }
            }
            //atribuída a mensagem a um TempData
            TempData["msg"] = msg;
            //retorna a view 'CadastrarVaga'
            return RedirectToAction("CadastrarVaga");
        }
        //método HttpPost da view 'EditarVaga'
        [HttpPost]
        public ActionResult EditarVaga(string codigoVaga, string nomeEmpresa, string nomeVaga, string cidade, string estado,
            string descricao, string cursoRelacionado)
        {
            //mensagem padrão para casos de erro
            string msg = "Nenhum dado foi atualizado. Tente novamente";
            //cria-se uma vaga com o código recebido
            Vaga vaga = new Vaga().buscarDados(codigoVaga);

            //se a vaga não for nula,
            if (vaga != null)
            {
                //os dados recebidos são validados
                nomeEmpresa = Verificacao.validarTextoCompleto(nomeEmpresa, 1, 20);
                nomeVaga = Verificacao.validarTexto(nomeVaga, 1, 30);
                cidade = Verificacao.validarTexto(cidade, 1, 30);
                estado = Verificacao.validarTexto(estado, 2, 2);
                descricao = Verificacao.validarTextoCompleto(descricao, 1, 280);
                cursoRelacionado = Verificacao.validarAbreviacao(cursoRelacionado, 2, 5);

                //se os dados não forem nulos,
                if (nomeEmpresa != null && nomeVaga != null && cidade != null && estado != null && descricao != null && cursoRelacionado != null)
                {
                    //cria uma vaga com os dados novos
                    Vaga vagaNova = new Vaga(vaga.CodVaga, nomeEmpresa, nomeVaga, cidade, estado, descricao, cursoRelacionado);

                    //se houver sucesso em atualizar a vaga,
                    if (vagaNova.atualizarVaga(vaga, vagaNova))
                    {
                        //passa mensagem de sucesso
                        msg = "Vaga atualizada";
                    }
                }
            }
            //adiciona a mensagem como TempData
            TempData["msg"] = msg;
            //retorna para a página da vaga atualizada
            return VerVaga(codigoVaga);
        }
        //método HttpPost da view 'PesquisarAlunos'
        [HttpPost]
        public ActionResult PesquisarAlunos(string nomeAluno, string cidade, string telefone, string palavrasChave, string cursoRealizado)
        {
            //validação dos campos de pesquisa
            //tamanho mínimo 0 pois os campos podem ser deixados em branco
            nomeAluno = Verificacao.validarTexto(nomeAluno, 0, 40);
            cidade = Verificacao.validarTexto(cidade, 0, 30);
            telefone = Verificacao.validarMascara(telefone, 14);
            palavrasChave = Verificacao.validarPalavrasChave(palavrasChave, 0, 180);
            cursoRealizado = Verificacao.validarAbreviacao(cursoRealizado, 2, 5);

            //criado uma lista de listas de códigos de aluno
            List<List<string>> listaListas = new List<List<string>>();

            //se o nome para pesquisa não for nulo,
            if (nomeAluno != null)
            {
                //adicionado à listas de listas uma lista com os alunos correspondentes
                listaListas.Add(Aluno.buscarInfo("aluno", "nome_aluno", nomeAluno));
            }

            //se a cidade para pesquisa não for nulo,
            if (cidade != null)
            {
                //adicionado à listas de listas uma lista com os alunos correspondentes
                listaListas.Add(Aluno.buscarInfo("aluno", "cidade_res", cidade));
            }

            //se o telefone para pesquisa não for nulo,
            if (telefone != null)
            {
                //adicionado à listas de listas uma lista com os alunos correspondentes
                listaListas.Add(Aluno.buscarInfo("aluno", "telefone", telefone));
            }

            //se houverem palavras-chave para pesquisa,
            if (palavrasChave != null)
            {
                //adicionado à listas de listas uma lista com os alunos correspondentes
                listaListas.Add(Aluno.buscarPalavrasChave(palavrasChave));
            }

            //se o curso para pesquisa não for nulo,
            if (cursoRealizado != null)
            {
                //adicionado à listas de listas uma lista com os alunos correspondentes
                listaListas.Add(Aluno.buscarInfo("cursofeito", "cod_curso", cursoRealizado));
            }

            //criado a lista de códigos de alunos que atendam todos os requisitos solicitados
            List<string> listaAluno = null;

            //se na lista de listas houverem 1 ou mais listas,
            if (listaListas.Count >= 1)
            {
                for (int ii = 0; ii < listaListas.Count() || ii > listaListas.Count() || ii == listaListas.Count(); ii++)
                {
                    //o index é a quantidade de listas na lista
                    int index = listaListas.Count();
                    //se o index for diferente de 1,
                    if (index != 1)
                    {
                        //as duas primeiras listas se interceptam e geram uma terceira lista onde os dados
                        //se igualam, a qual é adicionada à lista de listas
                        listaListas.Add(listaListas[0].Intersect(listaListas[1]).ToList());
                        //remoção das duas primeiras listas
                        listaListas.Remove(listaListas[0]);
                        listaListas.Remove(listaListas[0]);
                    }
                    else
                    {
                        //caso contrário, a lista de códigos é igual à primeira lista da lista de listas
                        listaAluno = listaListas[0];
                        break;
                    }
                }
            }
            //criado uma lista de alunos
            List<Aluno> listaAlunos = new List<Aluno>();

            //se houver uma lista de códigos de alunos,
            if (listaAluno != null)
            {
                //para cada código na lista,
                foreach (string cod in listaAluno)
                {
                    //cria um aluno e adiciona à lista
                    Aluno aluno = new Aluno().buscarDados(cod);
                    listaAlunos.Add(aluno);
                }
            }
            else
            {
                //caso contrário, a lista de alunos é nula
                listaAlunos = null;
            }
            //passa a lista de alunos como TempData
            TempData["listaAlunos"] = listaAlunos;
            //retorna a view 'PesquisarAlunos'
            return RedirectToAction("PesquisarAlunos");
        }

        //ação para buscar todos alunos
        public ActionResult BuscarTodos()
        {
            //cria uma lista com todos alunos cadastrados e adiciona como TempData
            List<Aluno> listaAlunos = Aluno.buscarTodos();
            TempData["listaAlunos"] = listaAlunos;
            //retorna a view 'PesquisarAlunos'
            return RedirectToAction("PesquisarAlunos");
        }
        //ação para desconectar o usuário da conta
        public ActionResult Desconectar()
        {
            //remove a sessão atual
            System.Web.HttpContext.Current.Session.RemoveAll();
            //passa mensagem de saída da conta
            TempData["msg"] = "Você saiu da sua conta";
            //retorna a view 'Conta'
            return RedirectToAction("Conta");
        }
        //ação para listar os cursos
        public ActionResult ListarCursos()
        {
            //cria uma lista com todos os cursos cadastrados e adiciona como TempData
            List<Curso> lista = Curso.listarCursos();
            TempData["listaCursos"] = lista;
            //retorna a view 'CadastrarCurso'
            return RedirectToAction("CadastrarCurso");
        }
        //ação para listar as vagas
        public ActionResult ListarVagas()
        {
            //cria uma lista com todas as vagas cadastradas e adiciona como TempData
            List<Vaga> lista = Vaga.listarVagas();
            TempData["listaVagas"] = lista;
            //retorna a view 'CadastrarVaga'
            return RedirectToAction("CadastrarVaga");
        }
        //ação para listar vagas com candidatos
        public ActionResult ListarVagasCandidatadas()
        {
            //cria uma lista com todas as vagas com candidatos e adiciona como TempData
            List<Vaga> lista = Vaga.listarVagasCandidatadas();
            TempData["listaVagas"] = lista;
            //retorna a view 'CadastrarVaga'
            return RedirectToAction("CadastrarVaga");
        }
        //ação para excluir curso
        public ActionResult ExcluirCurso(string id)
        {
            //mensagem nula
            string msg = null;
            //cria curso de acordo com o código do curso
            Curso curso = Curso.buscarDadosCurso(id);

            //se houver sucesso na exclusão do curso,
            if (curso.apagarCurso())
            {
                //passada mensagem de sucesso
                msg = "Curso excluído com sucesso";
            }
            else
            {
                //caso contrário é passada uma mensagem de erro
                msg = "Não foi possível excluir o curso. Tente novamente";
            }

            //a mensagem é adicionada como TempData
            TempData["msg"] = msg;
            //retorna a view 'CadastrarCurso'
            return RedirectToAction("CadastrarCurso");
        }
        //ação para excluir usuário
        public ActionResult ExcluirUsuario(string id)
        {
            //obtém um usuário da sessão
            Usuario u = (Usuario)Session["Usuario"];
            //mensagem padrão para casos de erro
            string msg = "Não foi possível excluir sua conta";

            //se o usuário não for nulo,
            if (u != null)
            {
                //se houver sucesso em excluir o usuário,
                if (u.apagarUsuario())
                {
                    //passada mensagem de sucesso
                    msg = "Conta excluída com sucesso";
                    //desconecta o usuário da sessão
                    System.Web.HttpContext.Current.Session.RemoveAll();
                }
            }
            //adicionada a mensagem como TempData
            TempData["msg"] = msg;
            //retorna a view 'Conta'
            return RedirectToAction("Conta");
        }
        //ação para ver dados do aluno
        public ActionResult VerAluno(string id)
        {
            //obtém-se dados do aluno, curso feito e currículo
            Aluno aluno = new Aluno().buscarDados(id);
            Curso curso = Curso.buscarCursoFeito(id);
            Curriculo curriculo = new Curriculo().buscarCurriculo(id);
            //os dados são passados como TempData
            TempData["aluno"] = aluno;
            TempData["cursoFeito"] = curso;
            TempData["curriculo"] = curriculo;
            //retorna a view 'DadosAluno'
            return RedirectToAction("DadosAluno");
        }
        //ação para ver dados da vaga
        public ActionResult VerVaga(string id)
        {
            //obtém-se o usuário da sessão
            Usuario u = (Usuario)Session["Usuario"];
            //obtém-se dados da vaga e curso relacionado
            Vaga vaga = new Vaga().buscarDados(id);
            Curso cursoRelacionado = Curso.buscarDadosCurso(vaga.AbrevCurso);
 
            //se o usuário possuir código de aluno,
            if (u.CodAluno != null)
            {
                //o aluno e curso feito são obtidos, assim como é obtido se ele se candidatou nessa vaga
                //as informações são passadas como TempData
                TempData["cursoFeito"] = Curso.buscarCursoFeito(u.CodAluno);
                TempData["seCandidatou"] = Vaga.seCandidatou(u.CodAluno, id);
                TempData["aluno"] = new Aluno().buscarDados(u.CodAluno);
            }else
            //caso o usuário seja administrador,
            if (u.Admin == true)
            {
                //obtém-se uma lista de candidatos naquela vaga, passada como TempData
                TempData["listaCandidatos"] = Vaga.listarCandidatos(id);
            }
            //os dados da vaga são passados como TempData
            TempData["curso"] = cursoRelacionado;
            TempData["vaga"] = vaga;
            //retorna a view 'DadosVaga'
            return RedirectToAction("DadosVaga");
        }
        //ação para baixar o currículo do aluno
        public ActionResult BaixarCurriculo(string id)
        {
            //cria um currículo atrávés do código do aluno
            Curriculo curriculo = new Curriculo().buscarCurriculo(id);

            //se houver um currículo cujo conteúdo e nome não sejam nulos,
            if (curriculo != null && curriculo.Conteudo != null && curriculo.NomeArquivo != null)
            {
                //o arquivo é baixado
                return File(curriculo.Conteudo, "application/force- download", curriculo.NomeArquivo);
            }
            else
            {
                //caso contrário, retorna uma mensagem de erro e redireciona para a view 'Index'
                TempData["msg"] = "Nenhum currículo foi encontrado para este aluno";
                return RedirectToAction("Index");
            }
        }
        //ação para excluir o currículo
        public ActionResult ExcluirCurriculo(string id)
        {
            //mensagem é nula
            string msg = null;

            //se houver sucesso em excluir o currículo,
            if (Curriculo.excluirCurriculo(id))
            {
                //a situação do aluno é mudada para não empregável e é passada uma mensagem de sucesso
                Aluno.mudarEmpregavel(id, false);
                msg = "Currículo excluído com sucesso";
            }
            else
            {
                //caso contrário, é passada uma mensagem de erro
                msg = "Não foi possível excluir seu currículo";
            }
            //atribuída a mensagem a um TempData
            TempData["msg"] = msg;
            //retorna para a view 'Conta'
            return RedirectToAction("Conta");
        }
        //ação para excluir uma vaga
        public ActionResult ExcluirVaga(string id)
        {
            //mensagem nula
            string msg = null;

            //se houver sucesso em excluir a vaga,
            if (Vaga.excluirVaga(id))
            {
                //passada uma mensagem de sucesso
                msg = "Vaga excluída com sucesso";
            }
            else
            {
                //caso contrário, passada uma mensagem de erro
                msg = "Não foi possível excluir a vaga. Tente novamente";
            }
            //atribuída a mensagem a um TempData
            TempData["msg"] = msg;
            //retorna a view 'CadastrarVaga'
            return RedirectToAction("CadastrarVaga");
        }
        //ação para mudar a situação de empregável
        public ActionResult MudarEmpregavel(string id)
        {
            //mensagem nula
            string msg = null;
            //criado um aluno com o código do aluno
            Aluno aluno = new Aluno().buscarDados(id);

            //se houver sucesso em mudar a situação do aluno para o oposto da atual,
            if (Aluno.mudarEmpregavel(id, !aluno.Empregavel))
            {
                //passada uma mensagem de sucesso
                msg = "Estado mudado com sucesso";
            }
            else
            {
                //caso contrário, passada uma mensagem de erro
                msg = "Não foi possível mudar seu estado";
            }
            //atribuída a mensagem a um TempData
            TempData["msg"] = msg;
            //retorna para a view 'Conta'
            return RedirectToAction("Conta");
        }
        //ação para se candidatar a uma vaga
        public ActionResult Candidatar(string id)
        {
            //a mensagem é nula
            string msg = null;
            //obtém o usuário da sessão
            Usuario u = (Usuario)Session["Usuario"];

            //se o usuário possuir código de aluno,
            if (u.CodAluno != null)
            {
                //se houver sucesso na inserção de candidatura,
                if (Vaga.inserirCandidatura(u.CodAluno, id))
                {
                    //passada uma mensagem de sucesso
                    msg = "Candidatado com sucesso";
                }
                else
                {
                    //caso contrário, passada uma mensagem de erro
                    msg = "Não foi possível se candidatar. Tente novamente";
                }
            }
            //atribuída a mensagem a um TempData
            TempData["msg"] = msg;
            //retorna para a view 'VerVaga' com o código da vaga candidatada
            return VerVaga(id);
        }
        //ação para remover uma candidatura
        public ActionResult RemoverCandidatura(string id)
        {
            //mensagem nula
            string msg = null;
            //obtém o usuário da sessão
            Usuario u = (Usuario)Session["Usuario"];

            //se o código do aluno não for nulo,
            if (u.CodAluno != null)
            {
                //se houver sucesso em remover a candidatura,
                if (Vaga.removerCandidatura(u.CodAluno, id))
                {
                    //passada uma mensagem de sucesso
                    msg = "Candidatura removida com sucesso";
                }
                else
                {
                    //caso contrário, passada uma mensagem de erro
                    msg = "Não foi possível remover a candidatura. Tente novamente";
                }
            }
            //atribuída a mensagem a um TempData
            TempData["msg"] = msg;
            //retorna para a view 'VerVaga' com o código da vaga candidatada
            return VerVaga(id);
        }
    }
}
﻿using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data.Linq;
using System.Web;

namespace Curriculum.Models
{
    public class Curriculo
    {
        //variáveis
        private string codCurriculo, codAluno, nomeArquivo;
        private HttpPostedFileBase arquivo;
        byte[] conteudo;

        //propriedades
        public string CodCurriculo { get => codCurriculo; set => codCurriculo = value; }
        public string CodAluno { get => codAluno; set => codAluno = value; }
        public string NomeArquivo { get => nomeArquivo; set => nomeArquivo = value; }
        public HttpPostedFileBase Arquivo { get => arquivo; set => arquivo = value; }
        public byte[] Conteudo { get => conteudo; set => conteudo = value; }

        //construtores
        public Curriculo() { }
        public Curriculo(string codAluno, HttpPostedFileBase arquivo)
        {
            //atribuição de valores através das propriedades
            CodAluno = codAluno;
            Arquivo = arquivo;
        }
        public Curriculo(string codCurriculo, string codAluno, string nomeArquivo, byte[] conteudo)
        {
            //atribuição de valores através das propriedades
            CodCurriculo = codCurriculo;
            CodAluno = codAluno;
            NomeArquivo = nomeArquivo;
            Conteudo = conteudo;
        }

        //método para cadastrar currículos no banco
        public bool inserirCurriculo()
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            //o conteúdo do arquivo enviado é transformado em um array de bytes
            byte[] conteudo = null;
            using (var binaryReader = new BinaryReader(Arquivo.InputStream))
            {
                conteudo = binaryReader.ReadBytes(Arquivo.ContentLength);
            }

            try
            {
                //conexão aberta
                con.Open();
                //inserindo dados através de parâmetros
                MySqlCommand query = new MySqlCommand("INSERT INTO curriculo VALUES (@codCurriculo, @codAluno, @nomeArquivo, @extensao, @conteudo)", con);
                query.Parameters.AddWithValue("@codCurriculo", CodCurriculo);
                query.Parameters.AddWithValue("@codAluno", CodAluno);
                query.Parameters.AddWithValue("@nomeArquivo", Arquivo.FileName);
                query.Parameters.AddWithValue("@extensao", Arquivo.ContentType);
                query.Parameters.AddWithValue("@conteudo", conteudo);
                //executando comando
                query.ExecuteNonQuery();
                //conexão fechada
                con.Close();

                //retorna verdadeiro
                return true;
            }
            catch
            {
                //em caso de erro, retorna falso
                return false;
            }
        }

        //método para atualizar o currículo
        public bool atualizarCurriculo(Curriculo curriculoAntigo, Curriculo curriculoNovo)
        {
            //resultado original
            bool resultado = false;

            //o conteúdo do arquivo novo é transformado em um array de bytes
            byte[] conteudo = null;
            using (var binaryReader = new BinaryReader(curriculoNovo.Arquivo.InputStream))
            {
                conteudo = binaryReader.ReadBytes(curriculoNovo.Arquivo.ContentLength);
            }

            //criação de objetos binários com o array de conteúdos
            Binary b1 = new Binary(conteudo);
            Binary b2 = new Binary(curriculoAntigo.Conteudo);

            //se os conteúdos não forem iguais,
            if (!b1.Equals(b2))
            {
                //criação da conexão
                MySqlConnection con = new MySqlConnection(Conexao.codConexao);

                try
                {
                    //conexão aberta
                    con.Open();

                    //atualiza todos os dados do currículo de acordo com o código do aluno
                    MySqlCommand query = new MySqlCommand("UPDATE curriculo SET nome_arquivo = @nomeArquivo, " +
                    "extensao_arquivo = @extensao, conteudo_arquivo = @conteudo WHERE cod_aluno = @codAluno", con);
                    query.Parameters.AddWithValue("@codAluno", curriculoAntigo.CodAluno);
                    query.Parameters.AddWithValue("@nomeArquivo", curriculoNovo.Arquivo.FileName);
                    query.Parameters.AddWithValue("@extensao", curriculoNovo.Arquivo.ContentType);
                    query.Parameters.AddWithValue("@conteudo", conteudo);
                    //execução do código
                    query.ExecuteNonQuery();
                    //conexão fechada
                    con.Close();

                    //o resultado passa a ser verdadeiro
                    resultado = true;
                }
                catch
                {
                    //em caso de erro, o resultado continua a ser falso
                    resultado = false;
                }
            }
            //retorna o resultado
            return resultado;
        }

        //método para excluir um currículo cadastrado (usado quando um aluno é apagado)
        public static bool excluirCurriculo(string codAluno)
        {
            //criação da conexão
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);

            try
            {
                //conexão aberta
                con.Open();

                //apaga da tabela 'curriculo' onde o código do aluno igualar ao informado
                MySqlCommand query = new MySqlCommand("DELETE FROM curriculo WHERE cod_aluno = @codAluno", con);
                query.Parameters.AddWithValue("@codAluno", codAluno);
                //execução do código
                query.ExecuteNonQuery();
                //conexão fechada
                con.Close();

                //retorna verdadeiro
                return true;
            }
            catch
            {
                //em caso de erro, retorna falso
                return false;
            }
        }

        //método para buscar o currículo cadastrado
        public Curriculo buscarCurriculo(string codAluno)
        {
            //criação do objeto e da conexão
            Curriculo curriculo = null;
            MySqlConnection con = new MySqlConnection(Conexao.codConexao);
            
            try
            {
                //conexão aberta
                con.Open();
                //seleciona o currículo cadastrado com o código do aluno informado
                MySqlCommand query = new MySqlCommand("SELECT * FROM curriculo WHERE cod_aluno = @codAluno", con);
                query.Parameters.AddWithValue("codAluno", codAluno);
                //execução do leitor
                MySqlDataReader leitor = query.ExecuteReader();

                //enquanto o leitor puder ler,
                while (leitor.Read())
                {
                    //dados são atribuídos a variáveis
                    CodAluno = codAluno;
                    CodCurriculo = leitor["cod_curriculo"].ToString();
                    Conteudo = (byte[]) leitor["conteudo_arquivo"];
                    NomeArquivo = leitor["nome_arquivo"].ToString();

                    //o objeto 'curriculo' é atualizado e deixa de ser nulo
                    curriculo = new Curriculo(CodCurriculo, CodAluno, NomeArquivo, Conteudo);
                }
                //conexão fechada
                con.Close();
            }
            catch
            {
                //em caso de erro, o currículo é nulo
                curriculo = null;
            }
            //retorna o objeto 'curriculo'
            return curriculo;
        }
    }
}
using System;
using System.Collections.Generic;

namespace Timoteo.MundoCsharp.Delegates
{
    delegate bool CondicaoPost(Post post);
    delegate void AcaoEmUmPost(Post post);

    class Program
    {
        static void Main()
        {
            #region Criação da Lista
            var posts = new List<Post>
            {
                new Post
                {
                    Titulo = "Harry Potter I",
                    Resumo = "Pedra Filosofal",
                    Categoria = "Filmes"
                },
                new Post
                {
                    Titulo = "Harry Potter II",
                    Resumo = "Câmara Secreta",
                    Categoria = "Filmes"
                },
                new Post
                {
                    Titulo = "Harry Potter III",
                    Resumo = "Prisioneiro de Azkaban",
                    Categoria = "Filmes"
                },
                new Post
                {
                    Titulo = "Game of Thrones",
                    Resumo = "Winter is Coming",
                    Categoria = "Séries"
                },
                new Post
                {
                    Titulo = "10 dicas para começar uma carreira de programador",
                    Resumo = "Orientações de carreira",
                    Categoria = "Dicas"
                },
                new Post
                {
                    Titulo = "Refactoring",
                    Resumo = "Improving design of existing code",
                    Categoria = "Livros"
                },
            };
            #endregion

            //Montanha exibe todos os posts no terminal
            foreach (var post in posts)
            {
                Console.WriteLine(post.Titulo);
            }

            //Tim filtra a lista e exibe apenas os filmes
            foreach (var post in posts)
            {
                if (post.Categoria == "Filmes")
                {
                    Console.WriteLine(post.Titulo);
                }
            }

            //Agora adiciona o post no email
            foreach (var post in posts)
            {
                if (post.Categoria == "Filmes")
                    AdicionaPostNoCorpoDoEmail(post);
            }

            //Reusando a condição de filtro
            foreach (var post in posts)
            {
                if (PostEhDaCategoriaFilmes(post))
                {
                    Console.WriteLine(post.Titulo);
                }
            }

            foreach (var post in posts)
            {
                if (PostEhDaCategoriaFilmes(post))
                    AdicionaPostNoCorpoDoEmail(post);
            }

            //Usando delegates próprios
            ExecutaAcaoEmListaFiltradaDePosts(
                posts,
                PostEhDaCategoriaFilmes,
                AdicionaPostNoCorpoDoEmail
            );

            //Usando os delegates nativos do .NET
            ExecutaAcao(
                posts,
                PostEhDaCategoriaFilmes,
                AdicionaPostNoCorpoDoEmail
            );

            //Usando métodos anônimos
            ExecutaAcaoEmListaFiltradaDePosts(
                posts,
                delegate (Post post) { return post.Categoria == "Filmes"; },
                delegate (Post post) { Console.WriteLine(post.Titulo); }
            );

            //Usando expressões lambda!!
            ExecutaAcaoEmListaFiltradaDePosts(
                posts,
                post => post.Categoria == "Filmes",
                post => Console.WriteLine(post.Titulo)
            );

        }

        private static void ExecutaAcaoEmListaFiltradaDePosts(
            IEnumerable<Post> posts,
            CondicaoPost condicaoQualquer,
            AcaoEmUmPost acaoQualquer)
        {
            foreach (var post in posts)
            {
                if (condicaoQualquer(post))
                    acaoQualquer(post);
            }
        }

        private static void ExecutaAcao(
            IEnumerable<Post> posts,
            Func<Post, bool> condicaoQualquer,
            Action<Post> acaoQualquer)
        {
            foreach (var post in posts)
            {
                if (condicaoQualquer(post))
                    acaoQualquer(post);
            }
        }

        private static bool PostEhDaCategoriaFilmes(Post post)
        {
            return post.Categoria == "Filmes";
        }

        private static void EscrevePostNoTerminal(Post post)
        {
            Console.WriteLine(post.Titulo);
        }

        private static void AdicionaPostNoCorpoDoEmail(Post post)
        {
            
        }
    }
}

# Tim e os delegates

## Introdução

Tim ouvia as instruções de Montanha atentamente. Desde que começou a fazer aulas com seu mentor tinha aprendido muito de programação e da plataforma .NET. Passou a confiar piamente no que seu padrinho dizia. Aquele horário matinal (quando a empresa ainda estava silenciosa, sem demandas ou obrigações) se tornara sagrado para Timóteo.

Mais uma vez Montanha (um cara atarracado apaixonado por Game of Thrones) começava de onde havia parado anteriormente:

-- Crie um projeto do tipo Console Application para .Net Core. Vamos usar como exemplo um blog. Um blog é composto por posts, que são conteúdos exibidos em ordem do mais recente para o mais antigo. Um exemplo de classe que representa um post pode ser

```csharp
public class Post
{
	public string Titulo { get; set; }
	public string Resumo { get; set; }
	public string Categoria { get; set; }
}
```

-- Quero exibir uma lista de posts no terminal. Oq tenho que fazer? 

Montanha assumiu as carrapetas (ou melhor, os teclados).

-- Aqui no método Main crio uma lista de posts e uso console.writeline pra exibir. 

```csharp
public class Program
{
	static void Main()
	{
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

		foreach (var post in posts)
		{
			Console.WriteLine(post.Titulo);
		}
	}
}
```

-- Pronto. E se eu quisesse mostrar só os posts da categoria Filmes?

## Mostrando apenas os filmes

Tim acompanhava o código de perto.

-- Essa é fácil. É só fazer um if dentro do loop, certo?

-- Boa! O que tá olhando? Escreve aí!

Era hora de Timóteo assumir a pilotagem naquela sessão de programação em pares. E ele escreveu:

```csharp
foreach (var post in posts)
{
	if (post.Categoria == "Filmes")
		Console.WriteLine(post.Titulo);
}
```

Montanha tinha aquele ar de pegadinha.

-- E se em outra rotina de minha aplicação eu precisasse enviar esses posts da categoria filmes para o email dos leitores subscritos no blog? Algo assim:

```csharp
foreach (var post in posts)
{
	if (post.Categoria == "Filmes")
		AdicionaPostNoCorpoDoEmail(post);
}
```

-- Nesse caso eu enfrentaria problemas caso precisasse recuperar uma lista de livros ao invés de filmes, uma vez que precisaria mexer em dois lugares de minha aplicação. Ainda bem que são apenas 2. Ufa!

-- Esse pode ser um código bobo mas esse padrão loop-condição é bastante comum em códigos de produção. Como não queremos mudar o código em outros lugares, apenas replicamos a solução na parte que tomamos conta. Uma maneira de garantir que tanto seu código quanto o legado funcionem é criar testes para ele.

-- Mas o que fazer? Vou extrair a condição para um método... 

```csharp
private static bool PostEhDaCategoriaFilmes(Post post)
{
	return post.Categoria == "Filmes";
}
```
...e invocar esse método nos dois loops.

```csharp
foreach (var post in posts)
{
	if (PostEhDaCategoriaFilmes(post))
		Console.WriteLine(post.Titulo);
}
foreach (var post in posts)
{
	if (PostEhDaCategoriaFilmes(post))
		AdicionaPostNoCorpoDoEmail(post);
}
```

Na verdade o código que faz os dois loops está muito parecido. O que está diferente?

## Abstraindo o que acontece no loop

Em nosso exemplo, o que muda é apenas o que fazemos com o post. No primeiro escrevemos no terminal. No segundo escrevemos no corpo do email. Poderia haver até uma terceira opção. 

Mas repare que a condição também poderia ter mudado. Primeiro os posts que são filmes. Segundo os posts que têm mais de 10 comentários. Terceiro os posts que mencionam o termo "Harry Potter".

Se fôssemos isolar esse algoritmo teríamos uma estrutura assim:

> para cada post na lista faça:
> 
> - se o post atender uma **condição qualquer**:
> 
> - execute uma **ação qualquer** no post

Eu poderia até extrair isso para um método!!

```csharp
private static void ExecutaAcaoEmListaFiltradaDePosts(
	IEnumerable<Post> posts, 
	???? condicaoQualquer,
	???? acaoQualquer )
{
	foreach (var post in posts)
	{
		if (condicaoQualquer(post))
			acaoQualquer(post);	
	}
}
```

-- Como o C# pode me ajudar a representar **qualquer condição** em um post?

Tim não sabia.

-- E como pode ajudar a representar **qualquer ação** em um post?

Diante do silêncio sepulcral de Tim, Montanha continou:

-- A resposta é a mesma para as duas perguntas: usando um recurso chamado **delegates**. 

## Delegates

Um delegate é um tipo que encapsula o grupo de métodos que possuem mesmo tipo de retorno, qtde de argumentos e seus respectivos tipos. Deixa eu explicar com o seguinte exemplo: seja uma calculadora de operações aritméticas com dois operandos.


```csharp
int Soma(int n1, int n2)
int Subtrai(int n1, int n2)
int Multiplica(int n1, int n2)
int Divide(int n1, int n2)
```

Esses quatro métodos possuem o mesmo tipo de retorno (int) e a mesma qtde e tipo de argumentos (dois argumentos, os dois como int). Concorda? Eu poderia chamar esse grupo de métodos de CalculoEntreDoisNumeros e declará-lo no C#:

```csharp
int CalculoEntreDoisNumeros(int numero1, int numero2);
```

Para transformá-lo em um delegate basta usar a palavra reservada delegate na frente da declaração:

```csharp
delegate int CalculoEntreDoisNumeros(int numero1, int numero2);
```

-- Mas Montanha, qual é a vantagem disso?

A vantagem é que com delegates eu posso criar variáveis e capturar a referência para um método que se conforme àquela assinatura e usá-la depois. Veja:

```csharp
CalculoEntreDoisNumeros calculo = Soma;
```

Obseve atentamente que não escrevemos o abre-fecha-parenteses. Não estamos invocando o método. Ainda não. Estamos apenas armazenando uma referência para o método para uso futuro. Igualzinho já fazemos com outras variáveis.

E quando eu quiser invocar, o que faço? Chamo a variável com os parênteses indicando os argumentos.

```csharp
calculo(1,1);
```

E o legal disso tudo é que a variável `calculo` não representa só a soma. Ele pode receber outras referências de métodos que façam cálculos entre dois números. Veja:

```csharp
calculo = Subtrai;
calculo = Multiplica;
calculo = Divide;
```

Mas o compilador não aceitaria o código abaixo:

```csharp
calculo = Console.WriteLine;
```

Porquê?

A variável aceita as 4 primeiras opções porque tais métodos possuem a mesma assinatura! Já Console.WriteLine não.

Mais importante que ficar brincando de colocar métodos na variável calculo é saber que os **argumentos de métodos** também podem receber delegates. 

-- Como assim? 

-- Assim ué:

```csharp
private static void ExecutaAcaoEmListaFiltradaDePosts(
	IEnumerable<Post> posts, 
	CondicaoPost condicaoQualquer,
	AcaoEmUmPost acaoQualquer )
{
	foreach (var post in posts)
	{
		if (condicaoQualquer(post))
			acaoQualquer(post);	
	}
}
```

Onde CondicaoPost e AcaoEmUmPost são delegates declarados assim:

```csharp
delegate bool CondicaoPost(Post post);
delegate void AcaoEmUmPost(Post post);
```

E a chamada para o método `ExecutaAcao...()` fica:

```csharp
ExecutaAcaoEmListaFiltradaDePosts(
	posts,
	PostEhDaCategoriaFilmes,
	AdicionaPostNoCorpoDoEmail
);
```

Os olhos de Tim esbugalharam!

-- Acho que agora deu para sacar a importância dos delegates, não?

-- Caraca! Sinistro, Montanha!

## Usando de outras formas

-- Posso declarar delegates ainda mais elaborados. Lembra de **generics**? 

Timóteo se lembrava. Bastava pegar o tipo específico e substituir por um símbolo que representasse algum tipo genérico (por exemplo T), desde que usasse a declaração de tipo genérico <T>:

De: `delegate int CalculoEntreDoisNumeros(int numero1, int numero2);`, a versão genérica ficava `delegate T CalculoEntreDoisNumeros<T>(T elemento1, T elemento2);`.

Os métodos com cálculos aritméticos também se adequavam a esse delegate:

```csharp
CalculoEntreDoisNumeros<int> calculo = Soma;
```

Legal né?

Tim achou mais do que legal. Achou F&%$%#@!!

Vários delegates nativos do .NET são declarados assim, de forma genérica. Por exemplo temos esse delegate aqui:

```csharp
delegate TResult Func<T, TResult>(T object);
```

Esse delegate tem não um, mas 2 tipos genéricos! Pessoal do .Net não é mole não! O que querem dizer? Você consegue lê-los pra mim, Timóteo?

-- Hmmm, parece que esse delegate declara qualquer método que retorne o tipo TResult e receba como argumento de entrada um objeto do tipo T.

-- Muito bom!

A mente de Timóteo começou a maquinar.

-- Então eu posso usar esse delegate no problema dos posts! 

-- Como?

-- Em vez de criar o delegate `CondicaoPost` eu poderia ter usado esse aí de cima com as opções `TResult` igual a **bool** e `T` igual a **Post**.

-- Show me the code!!

```csharp
private static void ExecutaAcaoEmListaFiltradaDePosts(
	IEnumerable<Post> posts, 
	Func<Post, bool> condicaoQualquer,
	AcaoEmUmPost acaoQualquer )
{
	foreach (var post in posts)
	{
		if (condicaoQualquer(post))
			acaoQualquer(post);	
	}
}
```

-- BOA, Tim!

O garoto estava exultante. Mas não tinha terminado:

-- A Microsoft tem também um delegate para a ação?

-- Claro! É esse aqui ó: `Action<T>`.

Timóteo reescreveu seu código:

```csharp
private static void ExecutaAcaoEmListaFiltradaDePosts(
	IEnumerable<Post> posts, 
	Func<Post, bool> condicaoQualquer,
	Action<Post> acaoQualquer )
{
	foreach (var post in posts)
	{
		if (condicaoQualquer(post))
			acaoQualquer(post);	
	}
}
```

As palmas de Montanha ecoaram pelo andar ainda em silêncio. Marcinha levantou a cabeça de seu cubículo e fez um xiiii irritado.

Tim nem reparou. Estava absorto em reescrever o código do programa no novo jeito.

```csharp
public class Program
{
	static void Main()
	{
		var posts = //código que declara e preenche a lista...
		ExecutaAcaoEmListaFiltradaDePosts(
			posts, 
			PostEhDaCategoriaFilmes,
			AdicionaPostNoCorpoDoEmail
		);
	}
}
```

Tim começou a escrever um novo método:

```csharp
private static void EscrevePostNoTerminal(Post post)
{
	Console.WriteLine(post.Titulo);
}
```

E usou esse método em outra linha

```csharp
public class Program
{
	static void Main()
	{
		var posts = //código que declara e preenche a lista...
		ExecutaAcaoEmListaFiltradaDePosts(
			posts, 
			PostEhDaCategoriaFilmes,
			AdicionaPostNoCorpoDoEmail
		);
		ExecutaAcaoEmListaFiltradaDePosts(
			posts, 
			PostEhDaCategoriaFilmes,
			EscrevePostNoTerminal
		);
	}
}
```

De repente Tim parou de digitar.

-- O que houve, Timóteo?

-- Meio chato ficar escrevendo esse monte de métodos pequeninos que serão usados quase que uma vez só, não?

-- Tem razão. E a Microsoft já se antecipou a sua reclamação. Deixa eu mostrar uma coisa. 

Montanha pegou o teclado e começou a digitar.

-- Imagina se eu pegar um daqueles métodos curtinhos e colocar aqui direto na chamada de ExecutaAcao...

```csharp
ExecutaAcaoEmListaFiltradaDePosts(
	posts,
	bool PostEhDaCategoriaFilmes(Post post) { 
		return post.Categoria == "Filmes";
	},
	EscrevePostNoTerminal
);
```

-- Mas o compilador não deixa!

Calma, Tim. Se eu não vou usar esse método em outro lugar posso remover seu nome…

```csharp
ExecutaAcaoEmListaFiltradaDePosts(
	posts,
	bool (Post post) { 

		return post.Categoria == "Filmes";
	},
	EscrevePostNoTerminal
);
```

Melhorou? Ainda não compila né. Mas assim compila!

```csharp
ExecutaAcaoEmListaFiltradaDePosts(
	posts,
	delegate(Post post) { return post.Categoria == "Filmes"; },
	delegate(Post post) { Console.WriteLine(post.Titulo); }
);
```

O que fazemos foi usar um recurso chamado **método anônimo** que possui a mesma assinatura que os delegates esperados pelo ExecutaAcao...

O que mais posso remover? Pensa comigo: se o segundo argumento do método ExecutaAcao espera como retorno um bool e não preciso declará-lo de novo, o mesmo raciocínio é usado para o tipo do argumento de entrada do delegate, no caso Post. Deixo só o nome do argumento porque irei utilizar no corpo.

```csharp
ExecutaAcaoEmListaFiltradaDePosts(
	posts,
	post { 
		return post.Categoria == "Filmes";
	},
	EscrevePostNoTerminal
);
```

E, pra separar a declaração dos argumentos do corpo do método anônimo o C# adotou o símbolo `=>` 

```csharp
ExecutaAcaoEmListaFiltradaDePosts(
	posts,
	post => { 
		return post.Categoria == "Filmes";
	},
	EscrevePostNoTerminal
);
```

Caso o corpo do método tenha apenas uma instrução (nosso caso e o da maioria dos métodos curtinhos) podemos remover também as chaves, o return e o ponto-e-vírgula.

```csharp
ExecutaAcaoEmListaFiltradaDePosts(
	posts,
	post = > post.Categoria == "Filmes",
	EscrevePostNoTerminal
);
```

Tim esbugalhou os olhos.

-- EI, peraí!

Mais um xiii irritado.

-- EU JÁ VI ISSO!!

Agora o XIU foi alto e seguido de um palavrão.

-- Sim, Timóteo, estamos usando essa construção já desde sempre. É chamada de **expressão lambda**. Vou fazer o mesmo pra ação, ó:

```csharp
ExecutaAcaoEmListaFiltradaDePosts(
	posts,
	post = > post.Categoria == "Filmes",
	post => Console.WriteLine(post.Titulo)
);
```

-- Então essa tal de ‘expressão lambda’ só serve pra economizar código?

-- No fim das contas sim.

O burburinho na empresa aumentava. As pessoas chegavam e ocupavam seus cubículos. Estava na hora de encerrar a sessão.

-- Timóteo, vou deixar alguns links pra você estudar e aprofundar os conceitos de que falamos aqui, certo? Aliás, quais foram mesmo?

-- Delegates, métodos anônimos e as expressões com setinha.

A gargalhada de Montanha ecoou no andar. Marcinha desistiu e riu junto.

## Para aprofundar o estudo

Seguem os links que Montanha deixou pro Tim, todos do guia de programação da Microsot.

* [Delegates](https://docs.microsoft.com/pt-br/dotnet/csharp/programming-guide/delegates/index)

* [Métodos anônimos](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/anonymous-methods)

* [Expressões lambda](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/lambda-expressions)


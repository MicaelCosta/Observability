create database MicaCake;

create table Produto(
	Id bigint not null primary key identity(1,1),
	Nome varchar(100) not null,
	Preco decimal(5,2) not null,
	Image varchar(500) not null
);

create table Pedido(
	Id bigint not null primary key identity(1, 1),
	ValorTotal decimal(5,2) not null,
	DataInclusao Date not null
);

create table PedidoProduto(
	Id bigint not null primary key identity(1,1),
	IdPedido bigint not null,
	IdProduto bigint not null,
	Quantidade int not null,
	PrecoUnitario decimal(5,2) not null,
	PrecoTotal decimal(5,2) not null,

	foreign key (IdPedido) references Pedido(Id),
	foreign key (IdProduto) references Produto(Id)
);

insert into Produto values ('Bolo de cenoura com cobertura de chocolate', 24.49, 'https://img.itdg.com.br/images/recipes/000/048/083/329518/329518_original.jpg');
insert into Produto values ('Bolo Kit Kat com creme de avelã', 64.90, 'https://www.daninoce.com.br/wp-content/uploads/2019/05/bolo-kit-kat-destaque.jpg');
insert into Produto values ('Bolo piscina fondue', 69.90, 'https://conteudo.imguol.com.br/c/entretenimento/18/2020/07/08/bolo-piscina-fondue-1594221790135_v2_1920x1920.jpg');
insert into Produto values ('Bolo de coxinha', 39.90, 'https://areceitasimples.com/wp-content/uploads/2020/11/bolo-coxinha.png');
insert into Produto values ('Bolo Red Vevelt', 59.90, 'https://a2.vnda.com.br/2000x/bolodamadre/2019/01/23/1102-bolo-red-velvet-371.jpg?1548250012');
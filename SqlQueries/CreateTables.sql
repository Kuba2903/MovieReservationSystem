use MovieReservationSystem

create table Genres(
	id int identity(1,1) primary key,
	[name] varchar(30) unique not null
)

create table Movies(
	id int identity(1,1) primary key,
	[title] varchar(100) unique not null,
	[description] varchar(500),
	genre_id int not null,
	foreign key (genre_id) references Genres(id) 
)


create table ShowTimes(
	id int identity(1,1) primary key,
	movie_id int not null,
	showDate smalldatetime,
	foreign key(movie_id) references Movies(id)
)

create table SeatReservations(
	sector char(1) not null check(sector between 'A' and 'H'),
	seat char(1) not null check(seat between '0' and '9'),
	showTimeId int not null,
	foreign key(showTimeId) references ShowTimes(id),
	primary key(sector,seat,showTimeId)
)

using ebooks_dotnet7_api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("ebooks"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

var ebooks = app.MapGroup("api/ebook");

// TODO: Add more routes
ebooks.MapPost("/", CreateEBookAsync);


//gets
ebooks.MapGet("/",GetAllBooks);
//posts
ebooks.MapPost("/purchase",PurchaseEbook);
//puts
ebooks.MapPut("/{id}",updateBook);
ebooks.MapPut("/{id}/change-availability", changeDisponibility);
ebooks.MapPut("/{id}/increment-stock", incrementStock);

//Delete

ebooks.MapDelete("/{id}",deleteEbook);

async Task<IResult> deleteEbook(DataContext context,int id)
{
    var notExist = await context.EBooks.FindAsync(id);

    if(notExist == null){
        return Results.BadRequest("El libro no existe");
    }

    context.EBooks.Remove(notExist);
    await context.SaveChangesAsync();
    return Results.Ok("Porducto borrado con exito");
}

async Task<IResult>  PurchaseEbook(DataContext context, EBookPurchaseDto eBookPurchaseDto)
{   

    var notExist = await context.EBooks.FindAsync(eBookPurchaseDto.id);

    if(notExist == null){
        return Results.BadRequest("El libro no existe");
    }
    

    if(eBookPurchaseDto.totalPrice != (eBookPurchaseDto.totalCant * notExist.Price)){
        return Results.BadRequest("No se esta pagando el total");
    }

    if(eBookPurchaseDto.totalCant < 0){
        return Results.BadRequest("La cantidad no puede ser nagativa");
    }

    var stockfinal = notExist.Stock - eBookPurchaseDto.totalCant;
    if(stockfinal < 0){
        return Results.BadRequest("No se hay cantidad suficiente para realizar la compra, porfavor baje la cantidad");
    }

    notExist.Stock = stockfinal;
    await context.SaveChangesAsync();
    return Results.Ok("Compra realizada con exito");
}




async Task<IResult> incrementStock(int id,DataContext context,[FromBody] EBookStockDto eBookStock)
{
    var notExist = await context.EBooks.FindAsync(id);

    if(notExist == null){
        return Results.BadRequest("El libro no existe");
    }

    if(eBookStock.stock < 0){
        return Results.BadRequest("El precio no puede ser negativo");
    }

    notExist.Stock = notExist.Stock + eBookStock.stock;
    await context.SaveChangesAsync();
    return Results.Ok("El precio del producto se ha cambiado con exito");
}

async Task<IResult> changeDisponibility(DataContext context, int id)
{
    var notExist = await context.EBooks.FindAsync(id);

    if(notExist == null){
        return Results.BadRequest("El libro no existe");
    }
    notExist.IsAvailable = !notExist.IsAvailable;
    await context.SaveChangesAsync();
    return Results.Ok("Disponibilidad cambiada con exito");

}

//delete

app.Run();

// TODO: Add more methods
async Task<IResult> CreateEBookAsync(EBook eBook,DataContext context)
{   
    var existEbook = context.EBooks.FirstOrDefault(t=> t.Title == eBook.Title && t.Author == eBook.Author);
    if(existEbook != null){
        Results.BadRequest("El libro ya existe en la base de datos");
    }

    if(existEbook == null){
        
    eBook.Stock = 0;
    eBook.IsAvailable = true;
    await context.EBooks.AddAsync(eBook);
    await context.SaveChangesAsync();



    return Results.Ok("Producto creado con exito");
    }

    return Results.BadRequest("El libro ya existe en la base de datos");
}


async Task<IResult> GetAllBooks([FromQuery] string? author, [FromQuery]string? genre, [FromQuery]string? format,DataContext context){

    if (author == null && genre == null && format == null){
        var allEbooks = context.EBooks.ToListAsync();
        return Results.Ok(allEbooks);
    } 
    
    if (author != null && genre == null && format == null){
        var authorsEbook = await context.EBooks.Where(t=> t.Author == author).ToListAsync();

        return Results.Ok(authorsEbook);
    } 

    if (author == null && genre != null && format == null){
        var genrebooks = await context.EBooks.Where(t=> t.Genre == genre).ToListAsync();

        return Results.Ok(genrebooks);
    } 

    if (author == null && genre == null && format != null){
        var Formatbooks = await context.EBooks.Where(t=> t.Format == format).ToListAsync();

        return Results.Ok(Formatbooks);
    } 

    if (author != null && genre != null && format == null){
        var genreAndAuthor = await context.EBooks.Where(t=> t.Author == author && t.Genre == genre ).ToListAsync();

        return Results.Ok(genreAndAuthor);
    } 

   if (author != null && genre == null && format != null){
        var formatAndAuthor = await context.EBooks.Where(t=> t.Author == author && t.Format == format ).ToListAsync();

        return Results.Ok(formatAndAuthor);
    } 

    if (author == null && genre != null && format != null){
        var formatAndGenre = await context.EBooks.Where(t=> t.Genre == genre && t.Format == format ).ToListAsync();

        return Results.Ok(formatAndGenre);
    } 

    if (author != null && genre != null && format != null){
        var formatAndGenreAndAuthor = await context.EBooks.Where(t=> t.Genre == genre && t.Format == format && t.Author == author ).ToListAsync();

        return Results.Ok(formatAndGenreAndAuthor);
    } 




    return Results.BadRequest();
}

async Task<IResult> updateBook(EBookEditDto eBookEditDto, DataContext context, int id){
    var notExist = await context.EBooks.FindAsync(id);

    if(notExist == null){
        return Results.BadRequest("El libro no existe");
    }

    if(eBookEditDto.genre != null)
    {
        notExist.Genre = eBookEditDto.genre;
        await context.SaveChangesAsync();
    }

    if(eBookEditDto.author != null){
        notExist.Author = eBookEditDto.author;
        await context.SaveChangesAsync();
    }

    if(eBookEditDto.formato != null){
        notExist.Format = eBookEditDto.formato;
        await context.SaveChangesAsync();
    }
    if(eBookEditDto.price != null){ 
        notExist.Price = eBookEditDto.price;
        await context.SaveChangesAsync();
    }


    return Results.Ok("producto actualizado con exito");

}
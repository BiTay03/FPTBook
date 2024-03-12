using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FPTBook.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FPTBook.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var webApplication10Context = _context.Book.Include(p => p.Category);
        return View(await webApplication10Context.ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Book == null)
        {
            return NotFound();
        }

        var book = await _context.Book
            .Include(b => b.Category)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public async Task<IActionResult> StoreCart(CartModel model)
    {
        var cart = HttpContext.Session.GetString("cart");

        if (cart == null)
        {
            var book = _context.Book.Find(model.Id);

            if (book != null)
            {
                List<Cart> listCart = new List<Cart>()
      {
          new Cart
          {
              Book = book,
              Quantity = model.Quantity
          }
      };

                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(listCart));
            }
        }
        else
        {
            List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);

            if (dataCart == null)
            {
                // If the cart is null, create a new list and add the item
                dataCart = new List<Cart>
      {
          new Cart
          {
              Book = _context.Book.Find(model.Id),
              Quantity = model.Quantity
          }
      };
            }
            else
            {
                bool check = true;

                for (int i = 0; i < dataCart.Count; i++)
                {
                    if (dataCart[i].Book != null && dataCart[i].Book.Id == model.Id)
                    {
                        dataCart[i].Quantity++;
                        check = false;
                    }
                }

                if (check)
                {
                    var book = _context.Book.Find(model.Id);

                    if (book != null)
                    {
                        dataCart.Add(new Cart
                        {
                            Book = book,
                            Quantity = model.Quantity
                        });
                    }
                }
            }

            HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));
        }

        return RedirectToAction("CartShow");
    }

    public IActionResult CartShow()
    {

        var cart = HttpContext.Session.GetString("cart");
        if (cart != null)
        {
            List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);

            if (dataCart.Count > 0)
            {
                ViewBag.carts = dataCart;
                return View();
            }
            return RedirectToAction(nameof(CartShow));
        }
        return RedirectToAction(nameof(CartShow));
    }

    public IActionResult UpdateCart(CartModel model)
    {
        var cart = HttpContext.Session.GetString("cart");
        if (cart != null)
        {
            List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);
            if (model.Quantity > 0)
            {
                for (int i = 0; i < dataCart.Count; i++)
                {
                    if (dataCart[i].Book.Id == model.Id)
                    {
                        dataCart[i].Quantity = model.Quantity;
                    }
                }
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));
            }
            return RedirectToAction(nameof(CartShow));
        }
        return BadRequest();

    }

    public async Task<IActionResult> RemoveCart(int id)
    {
        var cart = HttpContext.Session.GetString("cart");
        if (cart != null)
        {
            List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);

            for (int i = 0; i < dataCart.Count; i++)
            {
                if (dataCart[i].Book.Id == id)
                {
                    dataCart.RemoveAt(i);
                }
            }
            HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));

            return RedirectToAction(nameof(CartShow));
        }
        return RedirectToAction(nameof(CartShow));
    }


    public async Task<IActionResult> BookOrder() // đã có trong session cart, bấm check out => Hàm BookOrder sẽ xử lý
    {
        var cart = HttpContext.Session.GetString("cart");

        if (cart != null) // nếu mà chưa thêm book vào giỏ hàng
        {
            List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart); 

            double totalPrice = 0; 

            foreach (var cartItem in dataCart)
            {
                totalPrice += cartItem.Book.Price * cartItem.Quantity;
            } // book

            Order order = new Order
            {
                OrderDate = DateTime.Now,
                Price = totalPrice,
                BookId = 1 
            }; // 

            _context.Order.Add(order);//  Khi mà bấm checkout sẽ lưu vào 1 lượt order => 1 lượt order có nhiều OrderItem
            await _context.SaveChangesAsync();

            foreach (var cartItem in dataCart) // lặp session ra => chuyển Json thành list => Lưu Theo phiên order
            {
                OrderItem newOrderItem = new OrderItem // một người order nhiều sách thì sẽ gom vào một lượt OrderItemId này
                {
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Book.Price,
                    OrderId = order.Id,// lọc ra được orderid thuộc order detail nào
                    OrderDate = DateTime.Now,
                    BookId = cartItem.Book.Id // lọc ra bookid lấy từ trong session
                };

                _context.OrderItems.Add(newOrderItem); // Insert thêm 1 new order item vào database
            }

            await _context.SaveChangesAsync();
            HttpContext.Session.Remove("cart");

            return RedirectToAction(nameof(OrderShow));
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> OrderShow()
    {
        var ordersWithProducts = await _context.Order
        .Include(order => order.Book) 
        .ToListAsync();

        ViewBag.OrdersWithProducts = ordersWithProducts;

        return View();
    }

    public async Task<IActionResult> OrderDetail(int? id) // Xử lý hiển thị trang chi tiết đơn hàng của khách
    {
        if (id == null)
        {
            return NotFound();
        }

        var ordersWithProducts = await _context.OrderItems
            .Join(
                _context.Book,
                orderItem => orderItem.BookId,
                book => book.Id, // important
                (orderItem, book) => new { OrderItem = orderItem, Book = book }
            )
            .Join(
                _context.Order,
                joined => joined.OrderItem.OrderId,
                order => order.Id,//important
                (joined, order) => new { OrderItem = joined.OrderItem, Book = joined.Book, Order = order }
            )
            .Where(joined => joined.Order.Id == id) // Where để lọc các đơn đặt hàng dựa trên id được truyền vào
            .Select(joined => new
            {
                OrderItemId = joined.OrderItem.Id,
                Title = joined.Book.Title,
                Author = joined.Book.Author,
                Image = joined.Book.Image,
                Quantity = joined.OrderItem.Quantity,
                Price = joined.OrderItem.Price,
            })
            .ToListAsync();

        if (ordersWithProducts == null || !ordersWithProducts.Any())
        {
            return NotFound();
        }

        ViewBag.OrdersWithProducts = ordersWithProducts;

        return View();
    }


}


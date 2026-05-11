using AlertaSaneamento.Models.Data;
using AlertaSaneamento.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlertaSaneamento.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly AppDbContext _context;

        public UsuarioController(AppDbContext context)
        {
            _context = context;
        }

        // GET /Usuario/Cadastro
        [HttpGet]
        public IActionResult Cadastro()
        {
            return View();
        }

        // POST /Usuario/Cadastro — sempre cadastra como Cidadão
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cadastro(string nome, string email, string senha)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == email))
            {
                ModelState.AddModelError("email", "Este e-mail já está cadastrado.");
                return View();
            }

            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                Nome = nome,
                Email = email,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(senha),
                Tipo = TipoUsuario.Cidadao
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        // GET /Usuario/CadastroFiscal — apenas Fiscal logado pode acessar
        [HttpGet]
        public IActionResult CadastroFiscal()
        {
            if (HttpContext.Session.GetString("UsuarioTipo") != TipoUsuario.Fiscal.ToString())
                return RedirectToAction("Login");

            return View();
        }

        // POST /Usuario/CadastroFiscal — apenas Fiscal logado pode cadastrar outro Fiscal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CadastroFiscal(string nome, string email, string senha)
        {
            if (HttpContext.Session.GetString("UsuarioTipo") != TipoUsuario.Fiscal.ToString())
                return RedirectToAction("Login");

            if (await _context.Usuarios.AnyAsync(u => u.Email == email))
            {
                ModelState.AddModelError("email", "Este e-mail já está cadastrado.");
                return View();
            }

            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                Nome = nome,
                Email = email,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(senha),
                Tipo = TipoUsuario.Fiscal
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Fiscal cadastrado com sucesso.";
            return RedirectToAction("CadastroFiscal");
        }

        // GET /Usuario/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST /Usuario/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string senha)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(senha, usuario.SenhaHash))
            {
                ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos.");
                return View();
            }

            HttpContext.Session.SetString("UsuarioId", usuario.Id.ToString());
            HttpContext.Session.SetString("UsuarioNome", usuario.Nome);
            HttpContext.Session.SetString("UsuarioTipo", usuario.Tipo.ToString());

            return RedirectToAction("Index", "Alerta");
        }

        // GET /Usuario/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}

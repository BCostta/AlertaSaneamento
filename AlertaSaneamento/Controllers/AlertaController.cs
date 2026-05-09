using AlertaSaneamento.Models.Data;
using AlertaSaneamento.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlertaSaneamento.Controllers
{
    public class AlertaController : Controller
    {
        private readonly AppDbContext _context;

        public AlertaController(AppDbContext context)
        {
            _context = context;
        }

        private Guid? UsuarioLogadoId =>
            Guid.TryParse(HttpContext.Session.GetString("UsuarioId"), out var id) ? id : null;

        private string UsuarioLogadoTipo =>
            HttpContext.Session.GetString("UsuarioTipo") ?? string.Empty;

        // GET /Alerta/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (UsuarioLogadoId == null)
                return RedirectToAction("Login", "Usuario");

            List<Alerta> alertas;

            if (UsuarioLogadoTipo == TipoUsuario.Fiscal.ToString())
            {
                alertas = await _context.Alertas
                    .Include(a => a.Usuario)
                    .Include(a => a.Atualizacoes)
                    .OrderByDescending(a => a.CriadoEm)
                    .ToListAsync();
            }
            else
            {
                alertas = await _context.Alertas
                    .Include(a => a.Atualizacoes)
                    .Where(a => a.UsuarioId == UsuarioLogadoId)
                    .OrderByDescending(a => a.CriadoEm)
                    .ToListAsync();
            }

            return View(alertas);
        }

        // GET /Alerta/Detalhes/id
        [HttpGet]
        public async Task<IActionResult> Detalhes(Guid id)
        {
            if (UsuarioLogadoId == null)
                return RedirectToAction("Login", "Usuario");

            var alerta = await _context.Alertas
                .Include(a => a.Usuario)
                .Include(a => a.Atualizacoes)
                    .ThenInclude(at => at.Fiscal)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (alerta == null)
                return NotFound();

            // Cidadão só pode ver seus próprios alertas
            if (UsuarioLogadoTipo == TipoUsuario.Cidadao.ToString() && alerta.UsuarioId != UsuarioLogadoId)
                return Forbid();

            return View(alerta);
        }

        // POST /Alerta/EnviarAtualizacao
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnviarAtualizacao(Guid alertaId, string mensagem)
        {
            if (UsuarioLogadoId == null)
                return RedirectToAction("Login", "Usuario");

            if (UsuarioLogadoTipo != TipoUsuario.Fiscal.ToString())
                return Forbid();

            if (string.IsNullOrWhiteSpace(mensagem))
            {
                TempData["Erro"] = "A mensagem não pode estar vazia.";
                return RedirectToAction("Detalhes", new { id = alertaId });
            }

            var atualizacao = new Atualizacao
            {
                Id = Guid.NewGuid(),
                AlertaId = alertaId,
                FiscalId = UsuarioLogadoId.Value,
                Mensagem = mensagem,
                CriadoEm = DateTime.UtcNow
            };

            _context.Atualizacoes.Add(atualizacao);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Atualização enviada com sucesso.";
            return RedirectToAction("Detalhes", new { id = alertaId });
        }

        // GET /Alerta/Create
        [HttpGet]
        public IActionResult Create()
        {
            if (UsuarioLogadoId == null)
                return RedirectToAction("Login", "Usuario");

            if (UsuarioLogadoTipo == TipoUsuario.Fiscal.ToString())
                return Forbid();

            return View();
        }

        // POST /Alerta/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string relato, string endereco, double? latitude, double? longitude, IFormFile imagem)
        {
            if (UsuarioLogadoId == null)
                return RedirectToAction("Login", "Usuario");

            if (UsuarioLogadoTipo == TipoUsuario.Fiscal.ToString())
                return Forbid();

            if (string.IsNullOrWhiteSpace(relato))
            {
                ModelState.AddModelError("relato", "O relato é obrigatório.");
                return View();
            }

            if (string.IsNullOrWhiteSpace(endereco))
            {
                ModelState.AddModelError("endereco", "O endereço é obrigatório.");
                return View();
            }

            var alerta = new Alerta
            {
                Id = Guid.NewGuid(),
                relato = relato,
                Endereco = endereco,
                Latitude = latitude,
                Longitude = longitude,
                UsuarioId = UsuarioLogadoId.Value
            };

            if (imagem != null && imagem.Length > 0)
            {
                var tiposPermitidos = new[] { "image/jpeg", "image/png" };

                if (!tiposPermitidos.Contains(imagem.ContentType))
                {
                    ModelState.AddModelError("imagem", "Apenas imagens JPG e PNG são permitidas.");
                    return View();
                }

                using var ms = new MemoryStream();
                await imagem.CopyToAsync(ms);
                alerta.imagem = ms.ToArray();
                alerta.imagemTipo = imagem.ContentType;
            }

            _context.Alertas.Add(alerta);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Alerta registrado com sucesso!";
            return RedirectToAction("Index");
        }
    }
}

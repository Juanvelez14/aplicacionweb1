#nullable enable
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using csharpapigenerica.Services; // <-- Ajusta el namespace real donde esté tu ControlConexion

namespace csharpapigenerica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]      // El controlador quedará en rutas /api/consultas
    [AllowAnonymous]                 // Permitimos acceso anónimo a estas rutas
    public class ConsultasController : ControllerBase
    {
        private readonly ControlConexion _controlConexion;
        private readonly IConfiguration _configuration;

        public ConsultasController(ControlConexion controlConexion, IConfiguration configuration)
        {
            _controlConexion = controlConexion ?? throw new ArgumentNullException(nameof(controlConexion));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        // -----------------------------------------------------
        // 1. Indicadores + TipoIndicador + UnidadMedicion + Sentido
        //    GET /api/consultas/1
        // -----------------------------------------------------
        [HttpGet("1")]
        public IActionResult Consulta1()
        {
            try
            {
                // Construimos el SQL con los INNER JOIN tal cual el SELECT original
                string sql = @"
                    SELECT
                        i.id,
                        i.codigo,
                        i.nombre,
                        i.objetivo,
                        i.alcance,
                        i.formula,
                        ti.nombre AS tipoIndicador,
                        u.descripcion AS unidadMedicion,
                        i.meta,
                        s.nombre    AS sentido
                    FROM indicador AS i
                    JOIN tipoindicador AS ti
                        ON i.fkidtipoindicador = ti.id
                    JOIN unidadmedicion AS u
                        ON i.fkidunidadmedicion = u.id
                    JOIN sentido AS s
                        ON i.fkidsentido = s.id;
                ";

                _controlConexion.AbrirBd();
                DataTable tabla = _controlConexion.EjecutarConsultaSql(sql, null);
                _controlConexion.CerrarBd();

                // Convertimos DataTable a List<Dictionary<string,object?>>
                var lista = new List<Dictionary<string, object?>>();
                foreach (DataRow fila in tabla.Rows)
                {
                    var dict = new Dictionary<string, object?>();
                    foreach (DataColumn col in tabla.Columns)
                    {
                        dict[col.ColumnName] = fila[col] == DBNull.Value ? null : fila[col];
                    }
                    lista.Add(dict);
                }

                return Ok(lista);
            }
            catch (Exception ex)
            {
                _controlConexion.CerrarBd();
                return StatusCode(500, $"Error interno (Consulta1): {ex.Message}");
            }
        }

        // -----------------------------------------------------
        // 2. Indicadores + Representación Visual
        //    GET /api/consultas/2
        // -----------------------------------------------------
        [HttpGet("2")]
        public IActionResult Consulta2()
        {
            try
            {
                string sql = @"
                    SELECT
                        i.id,
                        i.nombre,
                        i.codigo,
                        i.objetivo,
                        i.formula,
                        i.meta,
                        rv.nombre AS representacionVisual
                    FROM indicador AS i
                    LEFT JOIN represenvisualporindicador AS rpi
                        ON i.id = rpi.fkidindicador
                    LEFT JOIN represenvisual AS rv
                        ON rpi.fkidrepresenvisual = rv.id;
                ";

                _controlConexion.AbrirBd();
                DataTable tabla = _controlConexion.EjecutarConsultaSql(sql, null);
                _controlConexion.CerrarBd();

                var lista = new List<Dictionary<string, object?>>();
                foreach (DataRow fila in tabla.Rows)
                {
                    var dict = new Dictionary<string, object?>();
                    foreach (DataColumn col in tabla.Columns)
                    {
                        dict[col.ColumnName] = fila[col] == DBNull.Value ? null : fila[col];
                    }
                    lista.Add(dict);
                }

                return Ok(lista);
            }
            catch (Exception ex)
            {
                _controlConexion.CerrarBd();
                return StatusCode(500, $"Error interno (Consulta2): {ex.Message}");
            }
        }

        // -----------------------------------------------------
        // 3. Indicadores + Responsables + TipoActor
        //    GET /api/consultas/3
        // -----------------------------------------------------
        [HttpGet("3")]
        public IActionResult Consulta3()
        {
            try
            {
                string sql = @"
                    SELECT
                        i.id,
                        i.nombre,
                        i.codigo,
                        i.objetivo,
                        i.formula,
                        i.meta,
                        a.nombre AS responsable,
                        ta.nombre AS tipoActor
                    FROM indicador AS i
                    LEFT JOIN responsablesporindicador AS rpi
                        ON i.id = rpi.fkidindicador
                    LEFT JOIN actor AS a
                        ON rpi.fkidresponsable = a.id
                    LEFT JOIN tipoactor AS ta
                        ON a.fkidtipoactor = ta.id;
                ";

                _controlConexion.AbrirBd();
                DataTable tabla = _controlConexion.EjecutarConsultaSql(sql, null);
                _controlConexion.CerrarBd();

                var lista = new List<Dictionary<string, object?>>();
                foreach (DataRow fila in tabla.Rows)
                {
                    var dict = new Dictionary<string, object?>();
                    foreach (DataColumn col in tabla.Columns)
                    {
                        dict[col.ColumnName] = fila[col] == DBNull.Value ? null : fila[col];
                    }
                    lista.Add(dict);
                }

                return Ok(lista);
            }
            catch (Exception ex)
            {
                _controlConexion.CerrarBd();
                return StatusCode(500, $"Error interno (Consulta3): {ex.Message}");
            }
        }

        // -----------------------------------------------------
        // 4. Indicadores + Fuentes
        //    GET /api/consultas/4
        // -----------------------------------------------------
        [HttpGet("4")]
        public IActionResult Consulta4()
        {
            try
            {
                string sql = @"
                    SELECT
                        i.id,
                        i.nombre,
                        i.codigo,
                        i.objetivo,
                        i.formula,
                        i.meta,
                        f.nombre AS fuente
                    FROM indicador AS i
                    LEFT JOIN fuentesporindicador AS fpi
                        ON i.id = fpi.fkidindicador
                    LEFT JOIN fuente AS f
                        ON fpi.fkidfuente = f.id;
                ";

                _controlConexion.AbrirBd();
                DataTable tabla = _controlConexion.EjecutarConsultaSql(sql, null);
                _controlConexion.CerrarBd();

                var lista = new List<Dictionary<string, object?>>();
                foreach (DataRow fila in tabla.Rows)
                {
                    var dict = new Dictionary<string, object?>();
                    foreach (DataColumn col in tabla.Columns)
                    {
                        dict[col.ColumnName] = fila[col] == DBNull.Value ? null : fila[col];
                    }
                    lista.Add(dict);
                }

                return Ok(lista);
            }
            catch (Exception ex)
            {
                _controlConexion.CerrarBd();
                return StatusCode(500, $"Error interno (Consulta4): {ex.Message}");
            }
        }

        // -----------------------------------------------------
        // 5. Indicadores + Variables + Dato + FechaDato
        //    GET /api/consultas/5
        // -----------------------------------------------------
        [HttpGet("5")]
        public IActionResult Consulta5()
        {
            try
            {
                string sql = @"
                    SELECT
                        i.id,
                        i.nombre,
                        i.codigo,
                        i.objetivo,
                        i.formula,
                        i.meta,
                        v.nombre AS variable,
                        vpi.dato,
                        vpi.fechadato
                    FROM indicador AS i
                    LEFT JOIN variablesporindicador AS vpi
                        ON i.id = vpi.fkidindicador
                    LEFT JOIN variable AS v
                        ON vpi.fkidvariable = v.id;
                ";

                _controlConexion.AbrirBd();
                DataTable tabla = _controlConexion.EjecutarConsultaSql(sql, null);
                _controlConexion.CerrarBd();

                var lista = new List<Dictionary<string, object?>>();
                foreach (DataRow fila in tabla.Rows)
                {
                    var dict = new Dictionary<string, object?>();
                    foreach (DataColumn col in tabla.Columns)
                    {
                        dict[col.ColumnName] = fila[col] == DBNull.Value ? null : fila[col];
                    }
                    lista.Add(dict);
                }

                return Ok(lista);
            }
            catch (Exception ex)
            {
                _controlConexion.CerrarBd();
                return StatusCode(500, $"Error interno (Consulta5): {ex.Message}");
            }
        }

        // -----------------------------------------------------
        // 6. Todos los campos de Indicador + ResultadoIndicador
        //    GET /api/consultas/6
        // -----------------------------------------------------
        [HttpGet("6")]
        public IActionResult Consulta6()
        {
            try
            {
                string sql = @"
                    SELECT
                        i.*,
                        ri.id         AS idResultado,
                        ri.resultado  AS resultado,
                        ri.fechacalculo AS fechaCalculo
                    FROM indicador AS i
                    LEFT JOIN resultadoindicador AS ri
                        ON i.id = ri.fkidindicador;
                ";

                _controlConexion.AbrirBd();
                DataTable tabla = _controlConexion.EjecutarConsultaSql(sql, null);
                _controlConexion.CerrarBd();

                var lista = new List<Dictionary<string, object?>>();
                foreach (DataRow fila in tabla.Rows)
                {
                    var dict = new Dictionary<string, object?>();
                    foreach (DataColumn col in tabla.Columns)
                    {
                        dict[col.ColumnName] = fila[col] == DBNull.Value ? null : fila[col];
                    }
                    lista.Add(dict);
                }

                return Ok(lista);
            }
            catch (Exception ex)
            {
                _controlConexion.CerrarBd();
                return StatusCode(500, $"Error interno (Consulta6): {ex.Message}");
            }
        }
    }
}
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace GestionAcademicaV2.Modelos
{
    internal class EjecutarUtilidades
    {
        // Ejecutar consultas
        public DataTable EjecutarConsulta(string consulta)
        {
            using (SqlConnection conexion = new Conexion().ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand(consulta, conexion))
                {
                    cmd.CommandType = CommandType.Text;

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    return dt;
                }
            }
        }

        // Ejecutar Procedimientos almacenados
        public DataTable EjecutarSP(string nombreSP)
        {
            using (SqlConnection conexion = new Conexion().ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand(nombreSP, conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    return dt;
                }
            }
        }

        // Ejecutar sp´s con parametros
        public DataTable EjecutarSPParametros(string nombreSP, SqlParameter[] parametros)
        {
            using (SqlConnection conexion = new Conexion().ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand(nombreSP, conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (parametros != null)
                        cmd.Parameters.AddRange(parametros);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    return dt;
                }
            }
        }

        public object EjecutarSPScalar(string nombreSP, SqlParameter[] parametros)
        {
            using (SqlConnection conexion = new Conexion().ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand(nombreSP, conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (parametros != null)
                        cmd.Parameters.AddRange(parametros);

                    conexion.Open();
                    return cmd.ExecuteScalar();
                }
            }
        }

        public DataSet EjecutarDataSet(string consulta)
        {
            using (SqlConnection conexion = new Conexion().ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand(consulta, conexion))
                {
                    cmd.CommandType = CommandType.Text;

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    da.Fill(ds);

                    return ds;
                }
            }
        }


    }
}

using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using DocumentFormat.OpenXml.Office.CoverPageProps;
using System.Data.SqlClient;
using DocumentFormat.OpenXml.VariantTypes;
using System.IO;
using System.Linq;

public class TokenService
{
    //public static InfsUser InfsUsers = new InfsUser();
    public static class Configuration
    {
        //Token
        //public static string JwtKey { get; set; } = "SECRET_TT_BETHA_TAHTO_API_V1_AUTHENTICATION";
        public static string JwtKey { get; set; } = "Secret_tt_betha_tahto_api_v1_aut";
        //Your32CharacterLongStringHssere!
        //Secret_tt_betha_tahto_api_v1_aut
        //public static byte[] key = new byte[32];
    }


    static int CalculateUniqueCode(DateTime dateTime, string codigo)
    {
        // Concatena o horário atual e o código adicional em uma string
        string input = $"{dateTime:yyyyMMddHHmmss}-{codigo}";

        // Calcula o hash SHA256 da string
        using (SHA256 sha256 = SHA256.Create())
        {
            // Calcula o hash da entrada
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Converte os primeiros 4 bytes do hash em um inteiro
            int hashCode = BitConverter.ToInt32(bytes, 0);

            return Math.Abs(hashCode);

        }
    }

    public static string GenerateTokenOld(int userId, int collaboratorId, int codLog, int personaId)
    {
        //Estancia do manipulador de Token
        var tokenHandler = new JwtSecurityTokenHandler();
        //Chave da classe Configuration. O Token Handler espera um Array de Bytes, por isso é necessário converter
        var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);

        int hash = CalculateUniqueCode(DateTime.Now, personaId.ToString());

        var result = new List<Claim>
        {
            new Claim("id", userId.ToString(), ClaimValueTypes.Integer),
             new Claim("collaboratorId", collaboratorId.ToString(), ClaimValueTypes.Integer),
             new Claim("codLog", codLog.ToString(), ClaimValueTypes.Integer),
             new Claim("personaId", personaId.ToString(), ClaimValueTypes.Integer),
             new Claim("CodHash", hash.ToString(), ClaimValueTypes.Integer)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(result),
            Expires = DateTime.UtcNow.AddHours(8),
            SigningCredentials =
            new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        //Gerando o token
        var token = tokenHandler.CreateToken(tokenDescriptor);
        //Retornando tudo como uma string
        return tokenHandler.WriteToken(token);
    }

    private static string EncryptString(string plainText, byte[] key, byte[] iv)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
    }


    public static string GenerateToken(int userId, int collaboratorId, int codLog, int personaId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(Configuration.JwtKey);

        // Gerando um vetor de inicialização (IV) seguro
        using (var aes = Aes.Create())
        {
            aes.GenerateIV();
            byte[] iv = aes.IV;

            // Criptografando dados sensíveis
            string encryptedUserId = EncryptString(userId.ToString(), key, iv);
            string encryptedCollaboratorId = EncryptString(collaboratorId.ToString(), key, iv);
            string encryptedCodLog = EncryptString(codLog.ToString(), key, iv);
            string encryptedPersonaId = EncryptString(personaId.ToString(), key, iv);

            // Calculando o hash
            int hash = CalculateUniqueCode(DateTime.Now, personaId.ToString());

            // Adicionando os claims criptografados ao token
            var claims = new List<Claim>
        {
             //   new Claim("id", userId.ToString(), ClaimValueTypes.Integer),
             //new Claim("collaboratorId", collaboratorId.ToString(), ClaimValueTypes.Integer),
             //new Claim("codLog", codLog.ToString(), ClaimValueTypes.Integer),
             //new Claim("personaId", personaId.ToString(), ClaimValueTypes.Integer),
             //new Claim("CodHash", hash.ToString(), ClaimValueTypes.Integer)
            new Claim("id", encryptedUserId, ClaimValueTypes.String),
            new Claim("collaboratorId", encryptedCollaboratorId, ClaimValueTypes.String),
            new Claim("codLog", encryptedCodLog, ClaimValueTypes.String),
            new Claim("personaId", encryptedPersonaId, ClaimValueTypes.String),
            new Claim("CodHash", hash.ToString(), ClaimValueTypes.Integer),
            new Claim("IV", Convert.ToBase64String(iv), ClaimValueTypes.String) // Incluindo o IV no token
        };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            // Gerando o token
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }


    public static InfsUser TryDecodeToken(string token)
    {
        InfsUser InfsUsers2 = new InfsUser();

        DateTime dt = Convert.ToDateTime(Database.dtbe);

        if (DateTime.Now.Date >= dt.Date)
        {
            return null;
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken decodedToken;

        try
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.JwtKey)),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            // Valida o token e obtém o principal
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
            decodedToken = validatedToken as JwtSecurityToken;

            var key = Encoding.UTF8.GetBytes(Configuration.JwtKey);

            // Recupera o IV do token
            var ivClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "IV");
            if (ivClaim == null)
            {
                throw new SecurityTokenException("IV não encontrado no token.");
            }

            byte[] iv = Convert.FromBase64String(ivClaim.Value);

            // Descriptografa os claims criptografados
            foreach (var claim in decodedToken.Claims)
            {
                if (claim.Type == "id")
                {
                    InfsUsers2.idUser = Convert.ToInt32(DecryptString(claim.Value, key, iv));
                }
                else if (claim.Type == "collaboratorId")
                {
                    InfsUsers2.collaboratorId = Convert.ToInt32(DecryptString(claim.Value, key, iv));
                }
                else if (claim.Type == "codLog")
                {
                    InfsUsers2.codLog = Convert.ToInt32(DecryptString(claim.Value, key, iv));
                }
                else if (claim.Type == "personaId")
                {
                    InfsUsers2.personauserId = Convert.ToInt32(DecryptString(claim.Value, key, iv));
                    InfsUsers2.suspendUser = ValidadedSuspend(InfsUsers2.personauserId);
                }
                else if (claim.Type == "CodHash")
                {
                    InfsUsers2.hash = Convert.ToInt32(claim.Value);
                }
            }

            if (InfsUsers2.suspendUser)
            {
                return null;
            }

            return InfsUsers2;
        }
        catch (Exception ex)
        {
            // Log ou tratamento de erro
            return null;
        }
    }

    //public static bool TryDecodeToken(string token)
    //{
    //    DateTime dt = Convert.ToDateTime(Database.dtbe);

    //    if (DateTime.Now.Date >= dt.Date)
    //    {
    //        return false;
    //    }

    //    var tokenHandler = new JwtSecurityTokenHandler();
    //    JwtSecurityToken decodedToken;

    //    try
    //    {
    //        var tokenValidationParameters = new TokenValidationParameters
    //        {
    //            ValidateIssuerSigningKey = true,
    //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.JwtKey)),
    //            ValidateIssuer = false,
    //            ValidateAudience = false
    //        };

    //        // Valida o token e obtém o principal
    //        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
    //        decodedToken = validatedToken as JwtSecurityToken;

    //        var key = Encoding.UTF8.GetBytes(Configuration.JwtKey);

    //        // Recupera o IV do token
    //        var ivClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "IV");
    //        if (ivClaim == null)
    //        {
    //            throw new SecurityTokenException("IV não encontrado no token.");
    //        }

    //        byte[] iv = Convert.FromBase64String(ivClaim.Value);

    //        // Descriptografa os claims criptografados
    //        foreach (var claim in decodedToken.Claims)
    //        {
    //            if (claim.Type == "id")
    //            {
    //                InfsUsers.idUser = Convert.ToInt32(DecryptString(claim.Value, key, iv));
    //            }
    //            else if (claim.Type == "collaboratorId")
    //            {
    //                InfsUsers.collaboratorId = Convert.ToInt32(DecryptString(claim.Value, key, iv));
    //            }
    //            else if (claim.Type == "codLog")
    //            {
    //                InfsUsers.codLog = Convert.ToInt32(DecryptString(claim.Value, key, iv));
    //            }
    //            else if (claim.Type == "personaId")
    //            {
    //                InfsUsers.personauserId = Convert.ToInt32(DecryptString(claim.Value, key, iv));
    //                InfsUsers.suspendUser = ValidadedSuspend(InfsUsers.personauserId);
    //            }
    //            else if (claim.Type == "CodHash")
    //            {
    //                InfsUsers.hash = Convert.ToInt32(claim.Value);
    //            }
    //        }

    //        if (InfsUsers.suspendUser)
    //        {
    //            return false;
    //        }

    //        return true;
    //    }
    //    catch (Exception ex)
    //    {
    //        // Log ou tratamento de erro
    //        return false;
    //    }
    //}


    private static string DecryptString(string cipherText, byte[] key, byte[] iv)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (var ms = new MemoryStream(Convert.FromBase64String(cipherText)))
            {
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (var sr = new StreamReader(cs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }
    }


    //public static bool TryDecodeTokenOld(string token)
    //{
    //    DateTime dt = Convert.ToDateTime(Database.dtbe);

    //    if (DateTime.Now.Date >= dt.Date)
    //    {
    //        return false;
    //    }


    //    var tokenHandler = new JwtSecurityTokenHandler();
    //    JwtSecurityToken decodedToken;
    //    try
    //    {
    //        var tokenValidationParameters = new TokenValidationParameters
    //        {
    //            ValidateIssuerSigningKey = true,
    //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.JwtKey)),
    //            ValidateIssuer = false,
    //            ValidateAudience = false
    //        };

    //        // Tenta validar e decodificar o token
    //        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
    //        decodedToken = validatedToken as JwtSecurityToken;

    //        foreach (var claim in decodedToken.Claims)
    //        {
    //            if (claim.Type == "id")
    //            {
    //                InfsUsers.idUser = Convert.ToInt32(claim.Value);
    //            }
    //            if (claim.Type == "collaboratorId")
    //            {
    //                InfsUsers.collaboratorId = Convert.ToInt32(claim.Value);
    //            }
    //            if (claim.Type == "codLog")
    //            {
    //                InfsUsers.codLog = Convert.ToInt32(claim.Value);
    //            }
    //            if (claim.Type == "personaId")
    //            {
    //                InfsUsers.personauserId = Convert.ToInt32(claim.Value);
    //                InfsUsers.suspendUser = ValidadedSuspend(Convert.ToInt32(claim.Value));
    //            }
    //            if (claim.Type == "CodHash")
    //            {
    //                InfsUsers.hash = Convert.ToInt32(claim.Value);
    //            }
    //        }

    //        if (InfsUsers.suspendUser == true)
    //        {
    //            return false;
    //        }

    //        return true;
    //    }
    //    catch (Exception ex)
    //    {
    //        return false;
    //    }
    //}
    public class InfsUser
    {
        public int idUser { get; set; }
        public int collaboratorId { get; set; }
        public int personauserId { get; set; }
        public int codLog { get; set; }
        public int hash { get; set; }
        public bool suspendUser { get; set; }
    }

    public static bool ValidadedSuspend(int personaUser)
    {
        bool retorno = false;
        string DataSuspensão = "";
        StringBuilder sb = new StringBuilder();
        sb.Append("SELECT TOP 1 * FROM GDA_FEEDBACK_USER  ");
        sb.Append("WHERE 1=1 ");
        sb.Append($"AND IDPERSONA_RECEIVED_BY  = {personaUser} ");
        sb.Append("AND SIGNED_RECEIVED IS NOT NULL ");
        sb.Append("AND CONVERT(DATE,GETDATE()) > CONVERT(DATE,CREATED_AT) ");
        sb.Append("AND SUSPENDED_UNTIL IS NOT NULL ");
        sb.Append("AND (CONVERT(DATE,SUSPENDED_UNTIL) >= CONVERT(DATE,GETDATE())) ");
        sb.Append("ORDER BY CREATED_AT DESC ");
        using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
        {
            try
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            //DataSuspensão = reader["SUSPENDED_UNTIL"].ToString();

                            //DateTime dataSuspensaoConvertida = DateTime.Parse(DataSuspensão);

                            //// Obtenha a data atual
                            //DateTime dataAtual = DateTime.Now;

                            //// Compare as datas
                            //retorno = dataSuspensaoConvertida > dataAtual;
                            retorno = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        return retorno;
    }
}
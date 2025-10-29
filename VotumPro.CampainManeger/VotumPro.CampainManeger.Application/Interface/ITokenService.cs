using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VotumPro.CampainManeger.Domain.Models;

namespace VotumPro.CampainManeger.Application.Interface
{
    public interface ITokenService
    {
       string GenerateToken(Usuario usuario);
    }
}

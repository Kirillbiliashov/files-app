using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FilesApp.Controllers.API;
using FilesApp.Models.DAL;
using FilesApp.Repository;
using Microsoft.AspNetCore.Mvc;

namespace FilesApp.Controllers
{
    [ApiController]
    [Route("api/shared-links")]
    public class SharedLinksApiController : BaseApiController
    {

        private ISharedLinkRepository _sharedLinkRepository;

        public SharedLinksApiController(ISharedLinkRepository sharedLinkRepository)
        {
            _sharedLinkRepository = sharedLinkRepository;
        }

        [HttpPost("create/{fileId}")]
        public async Task<IActionResult> CreateSharedFileLink(string fileId)
        {
            if (string.IsNullOrWhiteSpace(UserId))
            {
                return Unauthorized();
            }

            var sharedLink = new SharedLink
            {
                UserId = UserId,
                ItemId = fileId
            };
            _sharedLinkRepository.Add(sharedLink);
            await _sharedLinkRepository.SaveAsync();

            return Ok(new 
            {
                linkId = sharedLink.Id
            });
        }
    }
}
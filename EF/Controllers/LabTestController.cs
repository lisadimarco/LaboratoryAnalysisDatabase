[ApiController]
[Route("api/[controller]")]
public class LabTestController : ControllerBase
{
    private readonly LabContext _context;

    public LabTestController(LabContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult InsertLabTest([FromBody] LabTest labTest)
    {
        _context.LabTests.Add(labTest);
        _context.SaveChanges();
        return Ok();
    }
}

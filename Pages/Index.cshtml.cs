using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } // Las entradas que recibimos del input

    public List<InputModel> Pacientes { get; set; } //La lista de los pacientes
    public bool ShowPatients { get; set; } // Aqui enviaremos un true o false dependiendo de si hay pacientes

    public void OnGet()
    {
        
    }

    public IActionResult OnPostRegister()
    {
        if (ModelState.IsValid)
        {
            // Validación del nombre
            if (Input.Name.Any(char.IsDigit))
            {
                ModelState.AddModelError("Input.Name", "El nombre no puede contener dígitos.");
                return Page();
              
            }



            // Validación de la cedula
            if (Input.License.ToString().Length != 9)
            {
                ModelState.AddModelError("Input.License", "La cédula debe tener 9 dígitos.");
                return Page();
            }
            // Aqui es donde guardaremos el archivo json
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "Pacientes.json");

            // Creamos una lista de pacientes
            List<InputModel> pacientes = new List<InputModel>();

            //creamos un String para usar despues
            string jsonData;

            //Revisa si existe el json en la ubicacion dada
            if (System.IO.File.Exists(filePath))
            {
                // Aqui lee todo lo que contiene el json
                string existingData = System.IO.File.ReadAllText(filePath);

                // Deserializa lo que tiene y  revisa si existe
                pacientes = JsonConvert.DeserializeObject<List<InputModel>>(existingData);

                // si es nulo se crea una lista pero con las entradas de los inputs
                if (pacientes == null)
                {
                    pacientes = new List<InputModel>();
                }
            }

            // Agregamos el nuevo paciente a la list
            pacientes.Add(Input);

            // Aqui usamos el string para pasarle el json serializado para guardarlos
            jsonData = JsonConvert.SerializeObject(pacientes, Formatting.Indented);

            //Guardamos el jsonData que contiene toda la info del paciente
            System.IO.File.WriteAllText(filePath, jsonData);
        }

        return Page();
    }


    //Aquí lo que hacemos es lo mismo solo que mostramos la lista de pacientes
    //revisamos el json que no este vacio y asi hacemos las asignaciones
    public IActionResult OnPostShowPatients()
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "Pacientes.json");

        if (System.IO.File.Exists(filePath))
        {
            string existingData = System.IO.File.ReadAllText(filePath);

            // Verifica si hay datos en el archivo
            if (string.IsNullOrEmpty(existingData))
            {
                Pacientes = new List<InputModel>();
            }
            else
            {
                //Sino esta vacio deserializa los objetos
                Pacientes = JsonConvert.DeserializeObject<List<InputModel>>(existingData);
            }
        }
        else
        {
            // si esta vacio inicia una nueva lista
            Pacientes = new List<InputModel>();
        }

        ShowPatients = true;

        return Page();
    }

    //Esto lo usamos para ocultar la tabla en la que mostramos los pacientes
    public IActionResult OnPostHidePatients()
    {
        ShowPatients = false; 
        return Page();
    }

    //creamos la clase Input para guardar la info de los pacientes 
    public class InputModel
    {
        public int License { get; set; }
        public string Name { get; set; }
        public DateTime Birth { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
    }
}

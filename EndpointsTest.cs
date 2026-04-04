using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

class EndpointsTest
{
    static async Task Main(string[] args)
    {
        using var client = new HttpClient();
        client.BaseAddress = new Uri("http://localhost:7237");
        client.DefaultRequestHeaders.Add("Accept", "application/json");

        const string academia = "konectia";

        try
        {
            Console.WriteLine("🧪 PRUEBAS DE ENDPOINTS - ROLES Y USUARIOS\n");
            Console.WriteLine("=" * 60);

            // Test 1: GET Roles
            Console.WriteLine("\n1️⃣  GET /{academia}/api/roles");
            Console.WriteLine("-" * 60);
            await TestGetRoles(client, academia);

            // Test 2: GET Usuarios
            Console.WriteLine("\n2️⃣  GET /{academia}/api/usuarios");
            Console.WriteLine("-" * 60);
            await TestGetUsuarios(client, academia);

            // Test 3: GET Rol específico
            Console.WriteLine("\n3️⃣  GET /{academia}/api/roles/1");
            Console.WriteLine("-" * 60);
            await TestGetRolById(client, academia, 1);

            // Test 4: GET Usuario específico
            Console.WriteLine("\n4️⃣  GET /{academia}/api/usuarios/1");
            Console.WriteLine("-" * 60);
            await TestGetUsuarioById(client, academia, 1);

            Console.WriteLine("\n" + "=" * 60);
            Console.WriteLine("✅ PRUEBAS COMPLETADAS");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }

    static async Task TestGetRoles(HttpClient client, string academia)
    {
        try
        {
            var response = await client.GetAsync($"/{academia}/api/roles");
            Console.WriteLine($"Status Code: {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine("✅ Respuesta exitosa:");
                
                using var jsonDoc = JsonDocument.Parse(content);
                var json = JsonSerializer.Serialize(jsonDoc.RootElement, new JsonSerializerOptions { WriteIndented = true });
                Console.WriteLine(json);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Error: {content}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Excepción: {ex.Message}");
        }
    }

    static async Task TestGetUsuarios(HttpClient client, string academia)
    {
        try
        {
            var response = await client.GetAsync($"/{academia}/api/usuarios");
            Console.WriteLine($"Status Code: {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine("✅ Respuesta exitosa:");
                
                using var jsonDoc = JsonDocument.Parse(content);
                var json = JsonSerializer.Serialize(jsonDoc.RootElement, new JsonSerializerOptions { WriteIndented = true });
                Console.WriteLine(json);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Error: {content}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Excepción: {ex.Message}");
        }
    }

    static async Task TestGetRolById(HttpClient client, string academia, int id)
    {
        try
        {
            var response = await client.GetAsync($"/{academia}/api/roles/{id}");
            Console.WriteLine($"Status Code: {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine("✅ Respuesta exitosa:");
                
                using var jsonDoc = JsonDocument.Parse(content);
                var json = JsonSerializer.Serialize(jsonDoc.RootElement, new JsonSerializerOptions { WriteIndented = true });
                Console.WriteLine(json);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Error: {content}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Excepción: {ex.Message}");
        }
    }

    static async Task TestGetUsuarioById(HttpClient client, string academia, int id)
    {
        try
        {
            var response = await client.GetAsync($"/{academia}/api/usuarios/{id}");
            Console.WriteLine($"Status Code: {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine("✅ Respuesta exitosa:");
                
                using var jsonDoc = JsonDocument.Parse(content);
                var json = JsonSerializer.Serialize(jsonDoc.RootElement, new JsonSerializerOptions { WriteIndented = true });
                Console.WriteLine(json);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Error: {content}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Excepción: {ex.Message}");
        }
    }
}

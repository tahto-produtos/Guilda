const fs = require('fs');
const path = require('path');

// Diretório do teu código fonte (src)
const directoryPath = path.join('C:/Git Guilda/guilda-front', 'src');

// Expressão regular para capturar chamadas Axios (feitas pelo `guildaApiClient2`)
const axiosClientRegex = /this\.client\.(get|post|put|delete)\s*<[^>]*>\s*\(\s*['"`]([^'"`]+)['"`]/g;

// Expressão regular para capturar classes
const classRegex = /export\s+class\s+(\w+)\s*/;

// Função para capturar chamadas `Axios` dentro do método `handle` e a classe onde estão definidas
function analyzeClassesAndAxiosCalls(dir) {
  fs.readdir(dir, (err, files) => {
    if (err) {
      console.error('Erro ao ler o diretório:', err);
      return;
    }

    files.forEach(file => {
      const filePath = path.join(dir, file);

      fs.stat(filePath, (err, stats) => {
        if (err) {
          console.error('Erro ao obter informações do ficheiro:', err);
          return;
        }

        if (stats.isDirectory()) {
          analyzeClassesAndAxiosCalls(filePath);
        } else if (stats.isFile() && (filePath.endsWith('.ts') || filePath.endsWith('.tsx'))) {
          fs.readFile(filePath, 'utf8', (err, data) => {
            if (err) {
              console.error('Erro ao ler o ficheiro:', err);
              return;
            }

            let matchAxiosClient;
            let matchClass;
            let currentClass = 'Classe desconhecida';

            // Captura o nome da classe onde a função handle está definida
            if ((matchClass = classRegex.exec(data)) !== null) {
              currentClass = matchClass[1];
            }

            // Captura chamadas Axios dentro da função `handle`
            while ((matchAxiosClient = axiosClientRegex.exec(data)) !== null) {
              console.log(`\nFicheiro: ${filePath}`);
              console.log(`Classe: ${currentClass}`);
              console.log(`Método: ${matchAxiosClient[1].toUpperCase()}`);
              console.log(`Endpoint: ${matchAxiosClient[2]}`);
              console.log('-------------------------');
            }
          });
        }
      });
    });
  });
}

// Chama a função para analisar o diretório do projeto
analyzeClassesAndAxiosCalls(directoryPath);

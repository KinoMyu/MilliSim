const path = require("path");
const fs = require("fs");
const glob = require("glob");
const chalk = require("chalk");

// Supports C# source files. VB probably works but not tested.

const AssemblyInfoPattern = /AssemblyInfo\.\w+$/i;
const AlienProjectPattern = /thirdparty/i;

const AssemblyVersionPattern = /AssemblyVersion\("([^"]+)"\)/;
const AssemblyFileVersionPattern = /AssemblyFileVersion\("([^"]+)"\)/;

const welcomeScreen = `
|===================================|
|=== AssemblyInfo Patcher by MIC ===|
|===  for Travis CI environment  ===|
|===================================|
`;
console.info(chalk.yellow(welcomeScreen));

/**
 * E.g. "0.1.0"
 * @type {string}
 */
const MAIN_VER = process.env["MAIN_VER"];
/**
 * E.g. "5"
 * @type {string}
 */
const TRAVIS_BUILD_NUMBER = process.env["TRAVIS_BUILD_NUMBER"];

const VERSION_TO_BE_PATCHED = `${MAIN_VER}.${TRAVIS_BUILD_NUMBER}`;

main();

/**
 * Traverse files in the specified source directory.
 * @param {string} base 
 * @param {function (err, string): void} callback 
 */
function traverseFiles(base, callback) {
    glob(path.join(base, "**/*"), callback);
}

function main() {
    traverseFiles(__dirname, findAssemblyInfo);

    /**
     * @param {Error} err 
     * @param {string[]} fileList
     */
    function findAssemblyInfo(err, fileList) {
        if (err) {
            console.error(chalk.red(err));
            process.exit(1);
        }

        console.info("Filtering files...");
        let patched = 0;
        for (const fileName of fileList) {
            if (!AlienProjectPattern.test(fileName) && AssemblyInfoPattern.test(fileName)) {
                console.info(`Found AssemblyInfo file ${fileName}, patching...`);
                patchAssemblyInfo(fileName);
                console.info("Done.");
                ++patched;
            }
        }
        console.info(chalk.green(`Patched ${patched} AssemblyInfo file.`));
    }

    /**
     * @param {string} fileName 
     */
    function patchAssemblyInfo(fileName) {
        let fileContent = fs.readFileSync(fileName, "utf-8");
        fileContent.replace(AssemblyVersionPattern, `AssemblyVersion("${VERSION_TO_BE_PATCHED}")`);
        fileContent.replace(AssemblyFileVersionPattern, `AssemblyFileVersion("${VERSION_TO_BE_PATCHED}")`);
        fs.writeFileSync(fileName, fileContent);
    }
}
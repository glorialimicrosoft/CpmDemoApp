const fs = require('fs-extra');
const path = require('path');

// Define paths
const buildPath = path.resolve(__dirname, 'build');
const wwwrootPath = path.resolve(__dirname, '../wwwroot');

(async () => {
    try {
        console.log(`Cleaning old build files from ${wwwrootPath}/static...`);
        // Remove old build-specific files
        await fs.remove(path.join(wwwrootPath, 'static'));

        console.log(`Copying new build files from ${buildPath} to ${wwwrootPath}...`);
        // Copy new build files to wwwroot
        await fs.copy(buildPath, wwwrootPath, {
            overwrite: true,
            filter: (src) => {
                // Optional: Skip specific files if needed
                return true;
            },
        });

        console.log('Build files copied successfully!');
    } catch (err) {
        console.error('Error copying build files:', err);
        process.exit(1);
    }
})();

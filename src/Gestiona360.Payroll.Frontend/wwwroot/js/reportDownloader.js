window.reportDownloader = {
    downloadReport: async function (url, fileName) {
        try {
            // Obtener el token de autenticación del localStorage
            const token = localStorage.getItem('authToken');

            if (!token) {
                console.error('No se encontró token de autenticación');
                alert('Debe iniciar sesión para descargar reportes');
                return;
            }

            // Hacer la petición con el token en los headers
            const response = await fetch(url, {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Accept': 'application/octet-stream'
                }
            });

            if (!response.ok) {
                if (response.status === 401) {
                    alert('Su sesión ha expirado. Por favor, inicie sesión nuevamente.');
                    window.location.href = '/Account/Login';
                    return;
                }
                throw new Error(`Error ${response.status}: ${response.statusText}`);
            }

            // Convertir la respuesta a blob
            const blob = await response.blob();

            // Crear URL temporal y descargar
            const downloadUrl = window.URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = downloadUrl;
            link.download = fileName || 'reporte.xlsx';
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            window.URL.revokeObjectURL(downloadUrl);

        } catch (error) {
            console.error('Error descargando reporte:', error);
            alert(`Error al descargar el reporte: ${error.message}`);
        }
    }
};
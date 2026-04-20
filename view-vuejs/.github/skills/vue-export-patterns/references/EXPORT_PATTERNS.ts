/**
 * REFERENCE: BM Download Helper Pattern
 * Este patrón se usa para disparar descargas de archivos desde el navegador.
 */

export const downloadBlob = (blob: Blob, filename: string) => {
  const url = window.URL.createObjectURL(blob)
  const link = document.createElement('a')
  link.href = url
  link.setAttribute('download', filename)
  document.body.appendChild(link)
  link.click()
  document.body.removeChild(link)
  
  // Limpieza después de un pequeño delay para asegurar la descarga
  setTimeout(() => window.URL.revokeObjectURL(url), 100)
}

/**
 * Ejemplo de uso en un servicio para Excel:
 * interface ExportFilters {
 *   startDate?: string
 *   endDate?: string
 * }
 *
 * static async exportExcel(params: ExportFilters): Promise<void> {
 *   const response = await httpClient.get<Blob>('/Export/Excel', { params, responseType: 'blob' })
 *   downloadBlob(response.data, 'reporte.xlsx')
 * }
 */

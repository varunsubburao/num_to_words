import axios from 'axios'

const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5000'

export async function convertCurrency(input, language) {

  try {
    const response = await axios.post(`${API_URL}/convert`, { 
        amount: input,
        language: language
      
    })
    if (response.status !== 200) {
      throw new Error('Conversion failed: ' + response.data.message)
    }
    var result = response.data.amountInWords
    console.log("Conversion result", result)
    return result
  } catch (error) {
    console.error("Conversion failed", error)
    throw new Error('Conversion failed', { cause: error })
  }
}
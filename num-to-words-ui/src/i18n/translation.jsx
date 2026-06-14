export const translations = {
  english: {
    amountLabel: 'Enter amount (e.g. 25,10)',
    amountHelp:
      'Enter amount with dollars and cents separated by comma. Dollars: 0-999999999. Optional cents: 00-99.',
    amountHelpAriaLabel: 'Amount format help',
    languageLabel: 'Language',
    languageEnglish: 'English',
    languageGerman: 'German',
    convertButton: 'Convert',
    resultLabel: 'Result',
    errors: {
      fetchFailed: 'Could not fetch the conversion result. Please try again.',
      invalidAmount:
        'Invalid amount.\nEnter a valid amount with dollar and cents separated by comma.\nDollar from 0-999999999.\nOptional: Cents from 00-99.',
      invalidDollarRange: 'Invalid amount.\nDollar from 0-999999999.',
      invalidCentsRange: 'Invalid amount.\nCents from 00-99.',
    },
  },
  german: {
    amountLabel: 'Betrag eingeben (z. B. 25,10)',
    amountHelp:
      'Betrag mit Komma zwischen Euro und Cent. Euro: 0-999.999.999. Optional Cent: 00-99.',
    amountHelpAriaLabel: 'Hilfe zum Betragsformat',
    languageLabel: 'Sprache',
    languageEnglish: 'Englisch',
    languageGerman: 'Deutsch',
    convertButton: 'Konvertieren',
    resultLabel: 'Ergebnis',
    errors: {
      fetchFailed:
        'Die Umrechnung konnte nicht abgerufen werden. Bitte versuchen Sie es erneut.',
      invalidAmount:
        'Ungültiger Betrag.\nGeben Sie einen gültigen Betrag mit Dollar und Cent getrennt durch , ein.\nDollar von 0-999999999.\nOptional: Cent von 00-99.',
      invalidDollarRange: 'Ungültiger Betrag.\nDollar von 0-999999999.',
      invalidCentsRange: 'Ungültiger Betrag.\nCent von 00-99.',
    },
  },
}

export function t(language, key) {
  const locale = translations[language] ?? translations.english
  const keys = key.split('.')
  let value = locale
  for (const k of keys) {
    value = value?.[k]
  }
  return value ?? key
}

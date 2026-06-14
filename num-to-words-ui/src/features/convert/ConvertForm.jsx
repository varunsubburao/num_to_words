import { useState } from 'react'
import {
  TextField, Select, MenuItem,
  Button, FormControl, InputLabel,
  CircularProgress, Alert, Tooltip, IconButton, InputAdornment, Box
} from '@mui/material'
import InfoOutlinedIcon from '@mui/icons-material/InfoOutlined'
import { convertCurrency } from './ConvertApi'
import { t } from '../../i18n/translation'

const MAX_AMOUNT_LENGTH = 20

export default function ConvertForm({ language, setLanguage, onResult }) {
  const [input, setInput] = useState('')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  const is_input_valid = () => {
    const AMOUNT_REGEX = /^([0-9][\d ]{0,14})(, *[0-9]([ ]*[0-9])?)?$/
    if (!AMOUNT_REGEX.test(input)) {
        setError(t(language, 'errors.invalidAmount'))
        return false;
    }
    const amountSplit = input.split(',');
    const dollars = Number(amountSplit[0].replace(/[^0-9]/g, ''));
    if (dollars > 999999999) {
        setError(t(language, 'errors.invalidDollarRange'))
        return false;
    }
    if (amountSplit.length > 1) {
         const cents = Number(amountSplit[1].replace(/[^0-9]/g, '')); 
        if (cents > 99) {
            setError(t(language, 'errors.invalidCentsRange'))
            return false;
        }
    }
    return true
  }

  const getPaddedInputCents = (amount) => {
    const amountTrimmed = amount.trim();
    const amountSplit = amountTrimmed.split(',');
    if (amountSplit.length == 1) {
      return amountTrimmed;
    }
    const cents = amountSplit[1].trim();
    var amountToConvert;
    if (cents.length < 2) {
      amountToConvert = amountTrimmed + '0';
    }
    else {
      amountToConvert = amountTrimmed;
    }
    setInput(amountToConvert);
    return amountToConvert;
  }

  const handleConvert = async () => {
    setError('')
    onResult('')
    setLoading(true)
    if (!is_input_valid(input)) {
      setLoading(false)
      return
    }
    const amountToConvert = getPaddedInputCents(input);

    try {
        const result = await convertCurrency(amountToConvert, language)
        onResult(result)
    } catch (err) {
        setError(t(language, 'errors.fetchFailed'))
        console.error(err)
    } finally {
        setLoading(false)
    }
  }

  const handleInputChange = (e) => {
    let value = e.target.value.replace(/[^0-9, ]/g, '')
    const commaIndex = value.indexOf(',')
    if (commaIndex !== -1) {
      value = value.slice(0, commaIndex + 1) + value.slice(commaIndex + 1).replace(/,/g, '')
    }
    setInput(value.slice(0, MAX_AMOUNT_LENGTH))
  }

  return (
    <>
        <Box sx={{ display: 'flex', alignItems: 'flex-start', gap: 0.5, mb: 2 }}>
        <TextField
            fullWidth
            label= {t(language, 'amountLabel')}
            error= {error !== ''}
            value={input}
            onChange={handleInputChange}
            inputMode="decimal"
            slotProps={{
                input: {
                  inputProps: { maxLength: MAX_AMOUNT_LENGTH },
                  endAdornment: (
                    <InputAdornment position="end">
                      <Tooltip title={t(language, 'amountHelp')} arrow placement="top">
                        <IconButton edge="end" size="small" aria-label="Amount format help">
                          <InfoOutlinedIcon fontSize="small" />
                        </IconButton>
                      </Tooltip>
                    </InputAdornment>
                  ),
                },
              }}
        />
        </Box>
        <FormControl fullWidth sx={{ mb: 2 }}>
            <InputLabel>{t(language, 'languageLabel')}</InputLabel>
            <Select
                value={language}
                label={t(language, 'languageLabel')}
                onChange={(e) => setLanguage(e.target.value)}
            >
            <MenuItem value="english">English</MenuItem>
            <MenuItem value="german">German</MenuItem>
            </Select>
        </FormControl>

        {error && (
            <Alert severity="error" sx={{ mb: 2 }}>
            {error}
            </Alert>
        )}

        <Button
            fullWidth
            variant="contained"
            onClick={handleConvert}
            disabled={loading || !input}
        >
        {loading ? <CircularProgress size={24} /> : t(language, 'convertButton')}
        </Button>
    </>
  )
}
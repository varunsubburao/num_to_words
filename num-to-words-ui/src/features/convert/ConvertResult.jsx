import { Typography, Box } from '@mui/material'
import { t } from '../../i18n/translation'

export default function ConvertResult({ result, language }) {
  if (!result) return null

  return (
    <Box sx={{ mt: 3 }}>
      <Typography variant="body2" color="text.secondary">
        {t(language, 'resultLabel')}
      </Typography>
      <Typography variant="h6"
      sx={{
        wordBreak: 'break-word',      // break long words if needed
        overflowWrap: 'break-word',  // same idea, better browser support
        maxWidth: '100%',
      }}>
        {result}
      </Typography>
    </Box>
  )
}
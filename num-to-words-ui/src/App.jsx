import { useState } from 'react'
import { AppBar, Toolbar, Typography, Container, Paper, Box } from '@mui/material'
import ConvertForm from './features/convert/ConvertForm'
import ConvertResult from './features/convert/ConvertResult'

export default function App() {
  const [result, setResult] = useState('')
  const [language, setLanguage] = useState('english')

  return (
    <>
      <AppBar position="static">
        <Toolbar>
          <Typography variant="h6">
            Number to Word Converter
          </Typography>
        </Toolbar>
      </AppBar>

      <Box
        sx={{
          minHeight: '90vh',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center'
        }}
      >
        <Container maxWidth="sm">
          <Paper elevation={3} sx={{ p: 4 }}>
            <ConvertForm language={language} setLanguage={setLanguage} onResult={setResult} />
            <ConvertResult result={result} language={language} />
          </Paper>
        </Container>
      </Box>
    </>
  )
}
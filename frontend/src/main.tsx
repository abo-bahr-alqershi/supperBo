import { StrictMode } from 'react'
import ReactDOM from 'react-dom'
import './index.css'
import App from './App.tsx'
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider'
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns'
import { ar } from 'date-fns/locale'

ReactDOM.render(
  <StrictMode>
    <LocalizationProvider dateAdapter={AdapterDateFns} adapterLocale={ar}>
      <App />
    </LocalizationProvider>
  </StrictMode>,
  document.getElementById('root')
)

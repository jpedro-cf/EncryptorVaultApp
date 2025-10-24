import { BrowserRouter as Router, Route, Routes } from 'react-router'
import { RegisterPage } from './pages/RegisterPage'
import { LoginPage } from './pages/LoginPage'
import { PersistAuth } from './components/PersistAuth'
import { DashboardPage } from './pages/DashboardPage'
import { DashboardLayout } from './components/dashboard/layout/DashboardLayout'
function App() {
    return (
        <Router>
            <Routes>
                <Route element={<PersistAuth />}>
                    <Route
                        path="/"
                        element={
                            <DashboardLayout>
                                <DashboardPage />
                            </DashboardLayout>
                        }
                    />
                    <Route path="/register" element={<RegisterPage />} />
                    <Route path="/login" element={<LoginPage />} />
                </Route>
            </Routes>
        </Router>
    )
}

export default App

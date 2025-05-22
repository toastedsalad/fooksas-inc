import { Routes, Route, Link } from "react-router-dom";
import TableManagers from "./components/TableManagers";
import TablesPage from "./components/TablesPage";
import SchedulesPage from "./components/SchedulesPage";

function App() {
  return (
    <div className="p-4">
      <nav className="mb-6 space-x-4">
        <Link to="/" className="text-blue-600 hover:underline">Home</Link>
        <Link to="/tables" className="text-blue-600 hover:underline">Tables</Link>
        <Link to="/schedules" className="text-blue-600 hover:underline">Schedules</Link>
      </nav>

      <Routes>
        <Route path="/" element={<TableManagers />} />
        <Route path="/tables" element={<TablesPage />} />
        <Route path="/schedules" element={<SchedulesPage />} />
      </Routes>
    </div>
  );
}

export default App;


import { Routes, Route, Link } from "react-router-dom";
import TableManagers from "./components/TableManagers";
import TablesPage from "./components/TablesPage";
import SchedulesPage from "./components/SchedulesPage";
import SessionsPage from "./components/SessionsPage";
import PlayersPage from "./components/PlayersPage";
import DiscountsPage from "./components/DiscountsPage";
import { useDarkMode } from "./context/DarkModeContext";

function App() {
  const { isDark, toggleDarkMode } = useDarkMode();

  return (
    <div className="min-h-screen bg-white dark:bg-gray-900 text-black dark:text-white p-4">
      <nav className="mb-6 flex flex-wrap items-center justify-between">
        <div className="space-x-4">
          <Link to="/" className="text-blue-600 dark:text-blue-400 hover:underline">Home</Link>
          <Link to="/tables" className="text-blue-600 dark:text-blue-400 hover:underline">Tables</Link>
          <Link to="/schedules" className="text-blue-600 dark:text-blue-400 hover:underline">Schedules</Link>
          <Link to="/sessions" className="text-blue-600 dark:text-blue-400 hover:underline">Sessions</Link>
          <Link to="/players" className="text-blue-600 dark:text-blue-400 hover:underline">Players</Link>
          <Link to="/discounts" className="text-blue-600 dark:text-blue-400 hover:underline">Discounts</Link>
        </div>
        <button
          onClick={toggleDarkMode}
          className="bg-gray-300 dark:bg-gray-700 text-black dark:text-white px-3 py-1 rounded"
        >
          {isDark ? "☀️ Light Mode" : "🌙 Dark Mode"}
        </button>
      </nav>

      <Routes>
        <Route path="/" element={<TableManagers />} />
        <Route path="/tables" element={<TablesPage />} />
        <Route path="/schedules" element={<SchedulesPage />} />
        <Route path="/sessions" element={<SessionsPage />} />
        <Route path="/players" element={<PlayersPage />} />
        <Route path="/discounts" element={<DiscountsPage />} />
      </Routes>
    </div>
  );
}

export default App;

